using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
    /// Lógica interna para Detalhes.xaml
    /// </summary>
    public partial class GerarCobranca : Window
    {
        private SqlConnection connection; //Conexão com o sql server
        private SqlCommand command;       // Execução de comando no sql server
        private SqlDataReader lerDados; //Lê os resultados de um comando do sql executado no sql server

        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=custo;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private string query = string.Empty; //Usado para criar os comandos no sql

        private Modelos modelo;
        private List<Operacoes> listaOperacoes;
        private Operacoes operacao;
        private List<Operacoes> listaFase;

        public GerarCobranca()
        {
            InitializeComponent();           
        }

        public GerarCobranca(int oc) : this()
        {
            detalhes(oc);
            listaFase = new List<Operacoes>();
            DataGridGerarCobranca.ItemsSource = listaFase;
        }

        private void detalhes(int oc)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = @"SELECT mo.oc, mo.data_criacao, mo.referencia, mo.colecao, cli.cliente, mo.descricao
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

                                txtOC.Text = modelo.oc.ToString();
                                txtDataCriacao.Text = modelo.data_criacao;
                                txtReferencia.Text = modelo.referencia;
                                txtColecao.Text = modelo.colecao;
                                txtCliente.Text = modelo.cliente;
                                txtDescricao.Text = modelo.descricao;
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

                    query = @"SELECT fase, sum(total) as total FROM montar_custo WHERE oc = " + oc + "GROUP BY fase";


                    using (command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (lerDados = command.ExecuteReader())
                        {
                            while (lerDados.Read())
                            {
                                operacao = new Operacoes();

                                operacao.Fase = lerDados.GetString(0);
                                operacao.Custo = lerDados.GetDecimal(1);

                                listaOperacoes.Add(operacao);
                            }
                        }

                    }
                }

                DataGridOperacoesCobranca.ItemsSource = listaOperacoes;
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

        private void DataGridOperacoesCobranca_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridOperacoesCobranca.SelectedItem != null)
            {
                var operacao = (Operacoes)DataGridOperacoesCobranca.SelectedItem;

                txtProcesso.Text = operacao.Fase.ToString();
                txtPreco.Text = operacao.Custo.ToString();
                txtPercentual.Text = null;
            }
        }

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProcesso.Text) || string.IsNullOrWhiteSpace(txtPreco.Text))
            {
                MessageBox.Show("Não é possível incluir valores vazios");
            }
            else
            {
                operacao = new Operacoes();

                Decimal percentual;
                Decimal total;
                Decimal preco = Decimal.Parse(txtPreco.Text);

                if (string.IsNullOrWhiteSpace(txtPercentual.Text))
                {
                    percentual = 0;
                    total = preco;
                }
                else 
                {
                    percentual = Int32.Parse(txtPercentual.Text);
                    total = (preco * (percentual / 100)) + preco;
                }               

                operacao.Fase = txtProcesso.Text;
                operacao.Custo = preco;
                operacao.Percentual = percentual;
                operacao.Total = total;

                listaFase.Add(operacao);

                CollectionViewSource.GetDefaultView(DataGridGerarCobranca.ItemsSource).Refresh();                

                Soma(total);
            }
        }

        private void Soma(Decimal total)
        {
            Decimal preco;

            if (string.IsNullOrWhiteSpace(txtTotal.Text))
            {
                preco = total;
            }
            else
            {
                preco = total + Decimal.Parse(txtTotal.Text);
            }
            
            txtTotal.Text = preco.ToString();            
        }

        private void cbTipo_Loaded(object sender, RoutedEventArgs e)
        {
            cbTipo.Items.Add("PRODUÇÃO");
            cbTipo.Items.Add("MOSTRUÁRIO");
            cbTipo.Items.Add("PILOTO");
        }

        private void btnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTalao.Text) || string.IsNullOrWhiteSpace(txtTotal.Text) || string.IsNullOrWhiteSpace(cbTipo.Text))
            {
                MessageBox.Show("Não é possível finalizar, favor verificar os dados.");
            }
            else
            {
                try
                {
                    using (connection = new SqlConnection())
                    {
                        connection.ConnectionString = connectionString;

                        query = "INSERT INTO cobranca (talao, oc, dataCriacao, tipo, preco)  VALUES (@TALAO, @OC, @DATA, @TIPO, @PRECO)";

                        string data = DateTime.Now.ToString("dd/MM/yyyy");
                        string preco = txtTotal.Text;
                        preco = preco.Replace(',', '.');

                        command = new SqlCommand(query, connection);

                        command.Parameters.AddWithValue("@TALAO", txtTalao.Text);
                        command.Parameters.AddWithValue("@OC", txtOC.Text);
                        command.Parameters.AddWithValue("@DATA", data);
                        command.Parameters.AddWithValue("@TIPO", cbTipo.Text);
                        command.Parameters.AddWithValue("@PRECO", preco);

                        connection.Open();

                        command.ExecuteNonQuery();
                    }

                }
                catch (SqlException se)
                {
                    MessageBox.Show(se.Message);
                    this.Close();
                }
                finally
                {
                    connection.Close();
                    connection = null;
                    command = null;
                    MessageBox.Show("Cobrança finalizada com sucesso");
                    this.Close();
                }
            }
        }
    }
}
