using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Custo
{
    /// <summary>
    /// Lógica interna para Detahes.xaml
    /// </summary>
    public partial class Detahes : Window
    {

        private SqlConnection connection; //Conexão com o sql server
        private SqlCommand command;       // Execução de comando no sql server
        private SqlDataReader lerDados; //Lê os resultados de um comando do sql executado no sql server

        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=custo;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private string query = string.Empty; //Usado para criar os comandos no sql
        
        private Modelos modelo;
        private List<Operacoes> listaOperacoes;
        private Operacoes operacao;

        public Detahes()
        {
            InitializeComponent();
        }

        public Detahes(int oc) : this()
        {
            detalhar(oc);
        }

        private void detalhar(int oc)
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

                                txtOC.Text = modelo.oc.ToString();
                                txtDataCriacao.Text = modelo.data_criacao;
                                txtReferencia.Text = modelo.referencia;
                                txtColecao.Text = modelo.colecao;
                                txtCliente.Text = modelo.cliente;
                                txtDescricao.Text = modelo.descricao;
                                txtPreco.Text = modelo.preco.ToString();
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
                operacoes(oc);
            } 
        }

        public void operacoes(int oc)
        {
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

                                listaOperacoes.Add(operacao);
                            }
                        }

                    }                    
                }

                DataGridDetalhes.ItemsSource = listaOperacoes;
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
        }

        private void btnCobranca_Click(object sender, RoutedEventArgs e)
        {
            int oc = modelo.oc;

            var cobranca = new GerarCobranca(oc);

            cobranca.Show();
        }

        private void btnGerarPDF_Click(object sender, RoutedEventArgs e)
        {

            int oc = modelo.oc;
            GerarPDF pdf = new GerarPDF(oc);
        }
    }
}
