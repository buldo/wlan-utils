using System.Text;
using System.Text.RegularExpressions;

var projectDir = Directory.GetCurrentDirectory();
var policyFilePath = Path.Combine(projectDir, "nl80211_policy.txt");
var outputFilePath = Path.Combine(projectDir, "Nl80211AttributeParser.cs");

Console.WriteLine($"Reading policy file: {policyFilePath}");
Console.WriteLine($"Output file: {outputFilePath}");

if (!File.Exists(policyFilePath))
{
    Console.WriteLine($"Error: Policy file not found at {policyFilePath}");
    return;
}

var policyLines = File.ReadAllLines(policyFilePath);
var parsedAttributes = ParsePolicyFile(policyLines);

Console.WriteLine($"Parsed {parsedAttributes.Count} attributes");

var generatedCode = GenerateAttributeParser(parsedAttributes);

File.WriteAllText(outputFilePath, generatedCode);

Console.WriteLine($"Generated parser written to {outputFilePath}");

static List<AttributePolicy> ParsePolicyFile(string[] lines)
{
    var attributes = new List<AttributePolicy>();
    var linePattern = new Regex(@"^\[([A-Z0-9_]+)\]\s*=\s*(.+)$");

    // Attributes that don't exist in the Nl80211Attribute enum yet
    var skipAttributes = new HashSet<string>
    {
        "NL80211_ATTR_NAN_CONFIG",
        "NL80211_ATTR_BSS_PARAM",
        "NL80211_ATTR_S1G_PRIMARY_2MHZ"
    };

    foreach (var line in lines)
    {
        var trimmedLine = line.Trim();
        if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("//"))
            continue;

        var match = linePattern.Match(trimmedLine);
        if (!match.Success)
            continue;

        var attrName = match.Groups[1].Value;

        if (skipAttributes.Contains(attrName))
            continue;

        var policySpec = match.Groups[2].Value.TrimEnd(',');

        var policy = ParsePolicySpec(attrName, policySpec);
        if (policy != null)
        {
            attributes.Add(policy);
        }
    }

    return attributes;
}

static AttributePolicy? ParsePolicySpec(string attrName, string policySpec)
{
    // Handle different policy formats:
    // { .type = NLA_U32 }
    // NLA_POLICY_RANGE(NLA_U8, min, max)
    // NLA_POLICY_EXACT_LEN_WARN(len)
    // NLA_POLICY_MIN(NLA_U8, min)
    // NLA_POLICY_MAX(NLA_U8, max)
    // { .type = NLA_BINARY, .len = len }
    // { .type = NLA_NUL_STRING, .len = len }
    // NLA_POLICY_ETH_ADDR_COMPAT
    // NLA_POLICY_NESTED(policy)
    // NLA_POLICY_VALIDATE_FN(type, fn, len)
    // { .type = NLA_FLAG }
    // { .type = NLA_REJECT }
    // NLA_POLICY_FULL_RANGE(type, range)
    // NLA_POLICY_NESTED_ARRAY(policy)

    var policy = new AttributePolicy { AttributeName = attrName };

    // Handle macros
    if (policySpec.StartsWith("NLA_POLICY_RANGE"))
    {
        var typeMatch = Regex.Match(policySpec, @"NLA_POLICY_RANGE\(([A-Z0-9_]+)");
        if (typeMatch.Success)
        {
            policy.Type = typeMatch.Groups[1].Value;
            policy.HasValidation = true;
            policy.RawPolicy = policySpec;
        }
    }
    else if (policySpec.StartsWith("NLA_POLICY_EXACT_LEN_WARN") || policySpec.StartsWith("NLA_POLICY_EXACT_LEN"))
    {
        policy.Type = "NLA_BINARY";
        policy.HasValidation = true;
        policy.RawPolicy = policySpec;
    }
    else if (policySpec.StartsWith("NLA_POLICY_MIN") || policySpec.StartsWith("NLA_POLICY_MAX"))
    {
        var typeMatch = Regex.Match(policySpec, @"NLA_POLICY_(MIN|MAX)\(([A-Z0-9_]+)");
        if (typeMatch.Success)
        {
            policy.Type = typeMatch.Groups[2].Value;
            policy.HasValidation = true;
            policy.RawPolicy = policySpec;
        }
    }
    else if (policySpec.Contains("NLA_POLICY_ETH_ADDR"))
    {
        policy.Type = "NLA_BINARY";
        policy.HasValidation = true;
        policy.RawPolicy = policySpec;
    }
    else if (policySpec.StartsWith("NLA_POLICY_NESTED") || policySpec.StartsWith("NLA_POLICY_VALIDATE_FN"))
    {
        var typeMatch = Regex.Match(policySpec, @"NLA_POLICY_(?:NESTED|VALIDATE_FN)\(([A-Z0-9_]+)");
        if (typeMatch.Success)
        {
            policy.Type = typeMatch.Groups[1].Value;
        }
        else
        {
            policy.Type = "NLA_NESTED";
        }
        policy.HasValidation = true;
        policy.RawPolicy = policySpec;
    }
    else if (policySpec.StartsWith("NLA_POLICY_FULL_RANGE"))
    {
        var typeMatch = Regex.Match(policySpec, @"NLA_POLICY_FULL_RANGE\(([A-Z0-9_]+)");
        if (typeMatch.Success)
        {
            policy.Type = typeMatch.Groups[1].Value;
            policy.HasValidation = true;
            policy.RawPolicy = policySpec;
        }
    }
    else if (policySpec.Contains(".type"))
    {
        // Parse structured policy: { .type = NLA_U32 }
        var typeMatch = Regex.Match(policySpec, @"\.type\s*=\s*([A-Z0-9_]+)");
        if (typeMatch.Success)
        {
            policy.Type = typeMatch.Groups[1].Value;
        }

        var lenMatch = Regex.Match(policySpec, @"\.len\s*=\s*([A-Z0-9_\-\+\*\/\(\)]+)");
        if (lenMatch.Success)
        {
            policy.Length = lenMatch.Groups[1].Value;
        }

        policy.RawPolicy = policySpec;
    }
    else
    {
        // Unknown format, skip
        return null;
    }

    return policy;
}

static string GenerateAttributeParser(List<AttributePolicy> attributes)
{
    var sb = new StringBuilder();

    sb.AppendLine("// This file is auto-generated by Bld.Libnl.Generator");
    sb.AppendLine("// Do not edit manually");
    sb.AppendLine();
    sb.AppendLine("using System.Runtime.InteropServices;");
    sb.AppendLine("using Bld.Libnl.Types;");
    sb.AppendLine();
    sb.AppendLine("namespace Bld.Libnl;");
    sb.AppendLine();
    sb.AppendLine("public static partial class Nl80211AttributeParser");
    sb.AppendLine("{");
    sb.AppendLine("    /// <summary>");
    sb.AppendLine("    /// Parse nl80211 attribute based on its type");
    sb.AppendLine("    /// </summary>");
    sb.AppendLine("    public static unsafe INl80211AttributeValue? ParseAttribute(Nl80211Attribute attr, IntPtr nla)");
    sb.AppendLine("    {");
    sb.AppendLine("        if (nla == IntPtr.Zero)");
    sb.AppendLine("            return null;");
    sb.AppendLine();
    sb.AppendLine("        return attr switch");
    sb.AppendLine("        {");

    foreach (var policy in attributes)
    {
        var parseCode = GenerateParseCode(policy);
        sb.AppendLine($"            Nl80211Attribute.{policy.AttributeName} => {parseCode},");
    }

    sb.AppendLine("            _ => null");
    sb.AppendLine("        };");
    sb.AppendLine("    }");
    sb.AppendLine();

    // Generate individual helper methods for complex parsing
    sb.AppendLine("    private static unsafe INl80211AttributeValue? ParseBinary(IntPtr nla)");
    sb.AppendLine("    {");
    sb.AppendLine("        var len = LibNlNative.nla_len(nla);");
    sb.AppendLine("        if (len <= 0)");
    sb.AppendLine("            return null;");
    sb.AppendLine();
    sb.AppendLine("        var data = new byte[len];");
    sb.AppendLine("        var dataPtr = LibNlNative.nla_data(nla);");
    sb.AppendLine("        Marshal.Copy(dataPtr, data, 0, len);");
    sb.AppendLine("        return Nl80211AttributeValue.FromBinary(data);");
    sb.AppendLine("    }");
    sb.AppendLine();

    sb.AppendLine("    private static unsafe INl80211AttributeValue? ParseString(IntPtr nla)");
    sb.AppendLine("    {");
    sb.AppendLine("        var strPtr = LibNlNative.nla_get_string(nla);");
    sb.AppendLine("        if (strPtr == IntPtr.Zero)");
    sb.AppendLine("            return null;");
    sb.AppendLine();
    sb.AppendLine("        var str = Marshal.PtrToStringUTF8(strPtr);");
    sb.AppendLine("        return str != null ? Nl80211AttributeValue.FromString(str) : null;");
    sb.AppendLine("    }");
    sb.AppendLine();

    sb.AppendLine("    private static unsafe INl80211AttributeValue? ParseFlag(IntPtr nla)");
    sb.AppendLine("    {");
    sb.AppendLine("        // Flags are present or absent, if we have nla it means flag is set");
    sb.AppendLine("        return Nl80211AttributeValue.FromU8(1);");
    sb.AppendLine("    }");

    sb.AppendLine("}");

    return sb.ToString();
}

static string GenerateParseCode(AttributePolicy policy)
{
    // Map NLA types to parsing methods
    return policy.Type switch
    {
        "NLA_U8" => "Nl80211AttributeValue.FromU8(LibNlNative.nla_get_u8(nla))",
        "NLA_U16" => "Nl80211AttributeValue.FromU16(LibNlNative.nla_get_u16(nla))",
        "NLA_U32" => "Nl80211AttributeValue.FromU32(LibNlNative.nla_get_u32(nla))",
        "NLA_U64" => "Nl80211AttributeValue.FromU64(LibNlNative.nla_get_u64(nla))",
        "NLA_S8" => "Nl80211AttributeValue.FromS8(LibNlNative.nla_get_s8(nla))",
        "NLA_S16" => "Nl80211AttributeValue.FromS16(LibNlNative.nla_get_s16(nla))",
        "NLA_S32" => "Nl80211AttributeValue.FromS32(LibNlNative.nla_get_s32(nla))",
        "NLA_S64" => "Nl80211AttributeValue.FromS64(LibNlNative.nla_get_s64(nla))",
        "NLA_MSECS" => "Nl80211AttributeValue.FromMsecs(LibNlNative.nla_get_msecs(nla))",
        "NLA_STRING" or "NLA_NUL_STRING" => "ParseString(nla)",
        "NLA_BINARY" => "ParseBinary(nla)",
        "NLA_NESTED" => "Nl80211AttributeValue.FromNested(nla)",
        "NLA_FLAG" => "ParseFlag(nla)",
        "NLA_REJECT" => "null",
        _ => "ParseBinary(nla)"
    };
}

class AttributePolicy
{
    public string AttributeName { get; set; } = "";
    public string Type { get; set; } = "";
    public string? Length { get; set; }
    public bool HasValidation { get; set; }
    public string RawPolicy { get; set; } = "";
}
