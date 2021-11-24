using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data.SqlClient;
using System.Windows;
using System.Diagnostics;

namespace Custo
{
    class GerarPDF
    {
        private SqlConnection connection; //Conexão com o sql server
        private SqlCommand command;       // Execução de comando no sql server
        private SqlDataReader lerDados; //Lê os resultados de um comando do sql executado no sql server

        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=custo;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private string query = string.Empty; //Usado para criar os comandos no sql

        private Modelos modelo;
        private List<Operacoes> listaOperacoes;
        private Operacoes operacao;

        public GerarPDF(int oc)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = @"SELECT mo.oc, mo.data_criacao, mo.referencia, mo.colecao, cli.cliente, mo.descricao, mo.preco
                              FROM modelo mo, cliente cli 
                              WHERE mo.id_cliente = cli.id
                              AND oc = " + oc;

                    using (command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (lerDados = command.ExecuteReader())
                        {
                            while (lerDados.Read())
                            {
                                modelo = new Modelos();

                                modelo.oc = lerDados.GetInt32(0);
                                modelo.data_criacao = lerDados.GetString(1);
                                modelo.referencia = lerDados.GetString(2);
                                modelo.colecao = lerDados.GetString(3);
                                modelo.cliente = lerDados.GetString(4);
                                modelo.descricao = lerDados.GetString(5);
                                modelo.preco = lerDados.GetDecimal(6);
                            }
                        }
                    }
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }
            finally
            {
                connection.Close();
                connection = null;
                command = null;
            }

            string produto = modelo.referencia;

            Document doc = new Document(PageSize.A4);
            doc.SetMargins(40, 40, 40, 40);
            doc.AddCreationDate();
            string caminho = @"C:\pdf\" + produto + ".pdf";

            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(caminho, FileMode.Create));

            string dataCli = modelo.data_criacao +"  "+ modelo.cliente + " - " + modelo.colecao;

            string refDescPreco = modelo.referencia +" - "+ modelo.descricao +" - R$"+ modelo.preco;

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
           
            try
            {
                listaOperacoes = new List<Operacoes>();

                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = @"SELECT o.id, o.operacao, mc.freq, mc.total " +
                                    "FROM montar_custo mc, operacao o " +
                                    "WHERE mc.id_operacao = o.id " +
                                    "AND mc.oc = " + oc;
                    using (command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (lerDados = command.ExecuteReader())
                        {
                            while (lerDados.Read())
                            {
                                operacao = new Operacoes();

                                operacao.id = lerDados.GetInt32(0);
                                operacao.Operacao = lerDados.GetString(1);
                                operacao.Freq = lerDados.GetInt32(2);
                                operacao.Custo = lerDados.GetDecimal(3);

                                table.AddCell(operacao.id.ToString());
                                table.AddCell(operacao.Operacao);
                                table.AddCell(operacao.Freq.ToString());
                                table.AddCell(operacao.Custo.ToString());
                            }
                        }

                    }
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }
            finally
            {
                connection.Close();
                connection = null;
                command = null;
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
