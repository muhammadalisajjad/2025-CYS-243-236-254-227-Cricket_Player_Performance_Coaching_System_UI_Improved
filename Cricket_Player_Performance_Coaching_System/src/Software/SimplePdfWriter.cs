using System.Text;

namespace CricketPlayerManagementSystem.Software;

public static class SimplePdfWriter
{
    public static void WriteTextPdf(string path, string title, IEnumerable<string> lines)
    {
        var content = new StringBuilder();
        content.AppendLine("BT");
        content.AppendLine("/F1 18 Tf");
        content.AppendLine("50 780 Td");
        content.AppendLine($"({Escape(title)}) Tj");
        content.AppendLine("0 -28 Td");
        content.AppendLine("/F1 10 Tf");

        int count = 0;
        foreach (string rawLine in lines)
        {
            string line = rawLine.Length > 105 ? rawLine[..105] : rawLine;
            content.AppendLine($"({Escape(line)}) Tj");
            content.AppendLine("0 -15 Td");
            count++;
            if (count > 45) break;
        }
        content.AppendLine("ET");

        byte[] streamBytes = Encoding.ASCII.GetBytes(content.ToString());
        var objects = new List<string>
        {
            "<< /Type /Catalog /Pages 2 0 R >>",
            "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>",
            $"<< /Length {streamBytes.Length} >>\nstream\n{content}\nendstream"
        };

        var pdf = new StringBuilder();
        pdf.AppendLine("%PDF-1.4");
        var offsets = new List<int> { 0 };
        foreach (var obj in objects.Select((value, index) => new { value, index }))
        {
            offsets.Add(Encoding.ASCII.GetByteCount(pdf.ToString()));
            pdf.AppendLine($"{obj.index + 1} 0 obj");
            pdf.AppendLine(obj.value);
            pdf.AppendLine("endobj");
        }
        int xrefOffset = Encoding.ASCII.GetByteCount(pdf.ToString());
        pdf.AppendLine("xref");
        pdf.AppendLine($"0 {objects.Count + 1}");
        pdf.AppendLine("0000000000 65535 f ");
        foreach (int offset in offsets.Skip(1))
            pdf.AppendLine(offset.ToString("D10") + " 00000 n ");
        pdf.AppendLine("trailer");
        pdf.AppendLine($"<< /Size {objects.Count + 1} /Root 1 0 R >>");
        pdf.AppendLine("startxref");
        pdf.AppendLine(xrefOffset.ToString());
        pdf.AppendLine("%%EOF");

        File.WriteAllText(path, pdf.ToString(), Encoding.ASCII);
    }

    private static string Escape(string value)
    {
        return value.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
    }
}
