import jsPDF from "jspdf";

function stripHtml(html: string): string {
  return html
    .replace(/<\/p>/gi, "\n")
    .replace(/<br\s*\/?>/gi, "\n")
    .replace(/<\/li>/gi, "\n")
    .replace(/<[^>]+>/g, "")
    .replace(/&amp;/g, "&")
    .replace(/&lt;/g, "<")
    .replace(/&gt;/g, ">")
    .replace(/&nbsp;/g, " ")
    .trim();
}

export function exportNoteToPDF(title: string, content: string) {
  const pdf = new jsPDF();
  const margin = 10;
  let y = 20;

  pdf.setFontSize(16);
  pdf.text(title || "Untitled Note", margin, y);

  y += 10;
  pdf.setFontSize(11);

  const lines = pdf.splitTextToSize(stripHtml(content), 180);
  pdf.text(lines, margin, y);

  pdf.save(`${title || "note"}.pdf`);
}
