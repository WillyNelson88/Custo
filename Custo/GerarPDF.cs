using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Windows;
using System.Diagnostics;

namespace Custo
{
    class GerarPDF
    {
        
        private Modelo modelo;
        private List<Processo> listaProcessos;
        private Banco dataBase;

        public GerarPDF(int oc)
        {  
            dataBase = new();
            modelo = new();

            modelo = dataBase.Detalhes(oc);

            string produto = modelo.Referencia;

            Document doc = new Document(PageSize.A4);
            doc.SetMargins(40, 40, 40, 40);
            doc.AddCreationDate();
            string caminho = @"C:\pdf\" + produto + ".pdf";

            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(caminho, FileMode.Create));

            string dataCli = modelo.Data_criacao +"  "+ modelo.Cliente + " - " + modelo.Colecao;

            string refDescPreco = modelo.Referencia +" - "+ modelo.Descricao +" - R$"+ modelo.Preco;

            doc.Open();

            Paragraph first = new Paragraph();
            first.Font = FontFactory.GetFont("fontFamily", 12, Font.BOLD);
            first.Alignment = Element.ALIGN_CENTER;
            first.Add(dataCli + "\n\n");
            doc.Add(first);

            Paragraph second = new Paragraph();
            second.Font = FontFactory.GetFont("fontFamily", 12, Font.BOLD);
            second.Alignment = Element.ALIGN_CENTER;
            second.Add(refDescPreco + "\n\n");
            doc.Add(second);

            float[] columnWidths = { 5, 60, 5, 10 };
            PdfPTable table = new PdfPTable(4);
            table.SetWidths(columnWidths);

            PdfPCell id = new PdfPCell(new Phrase("ID", new Font(Font.FontFamily.COURIER, 14, Font.BOLD)));            
            PdfPCell oper = new PdfPCell(new Phrase("OPERAÇÃO", new Font(Font.FontFamily.COURIER, 14, Font.BOLD)));            
            PdfPCell x = new PdfPCell(new Phrase("X", new Font(Font.FontFamily.COURIER, 14, Font.BOLD)));            
            PdfPCell preco = new PdfPCell(new Phrase("PREÇO", new Font(Font.FontFamily.COURIER, 14, Font.BOLD)));

            oper.HorizontalAlignment = 1;

            table.AddCell(id);
            table.AddCell(oper);
            table.AddCell(x);
            table.AddCell(preco);
            
            listaProcessos = new List<Processo>();

            dataBase = new();

            listaProcessos = new List<Processo>();
            listaProcessos = dataBase.GetOperacaoModelo(oc);

            foreach (Processo processo in listaProcessos)
            {
                table.AddCell(processo.Id.ToString());
                table.AddCell(processo.Operacao);
                table.AddCell(processo.Freq.ToString());
                table.AddCell(processo.Custo.ToString());
            }


            doc.Add(table);

            string dia = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            string rodape = "Usuário: " + Sessao.SessaoUsuario + " - Gerado em: " + dia;
           

            Paragraph last = new Paragraph();
            last.Font = new Font(Font.FontFamily.COURIER, 9);
            last.Alignment = Element.ALIGN_CENTER;
            last.Add(rodape + "\n\n");
            doc.Add(last);

            doc.Close();

            new Process { StartInfo = new ProcessStartInfo(caminho) { UseShellExecute = true } }.Start();
        }
        
    }
}
