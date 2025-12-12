import jsPDF from "jspdf";

export function exportNoteToPDF(title: string, content: string) {
  const pdf = new jsPDF();
  const margin = 10;
  let y = 20;

  pdf.setFontSize(16);
  pdf.text(title || "Untitled Note", margin, y);

  y += 10;
  pdf.setFontSize(11);

  const lines = pdf.splitTextToSize(content, 180);
  pdf.text(lines, margin, y);

  pdf.save(`${title || "note"}.pdf`);
}
