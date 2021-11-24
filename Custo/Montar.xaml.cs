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
    /// Lógica interna para Montar.xaml
    /// </summary>
    public partial class Montar : Window
    {

        //ADO.NET
        private SqlConnection connection; //Conexão com o sql server
        private SqlCommand command;       // Execução de comando no sql server
        private SqlDataReader lerDados; //Lê os resultados de um comando do sql executado no sql server

        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=custo;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private string query = string.Empty; //Usado para criar os comandos no sql

        private List<Operacoes> listaProcessos;
        private List<Cliente> listaCliente;
        private Operacoes operacao;
        private List<Operacoes> listaOperacoesSelecionadas;

        public Montar()
        {
            InitializeComponent();
            readFromDatabase(null);

            listaOperacoesSelecionadas = new List<Operacoes>();
            GridMontar.ItemsSource = listaOperacoesSelecionadas;
        }

        //Ler dados da tabela e exibir na grid
        private void readFromDatabase(string where)
        {
            try
            {

                listaProcessos = new List<Operacoes>();

                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    if (where != null)
                    {
                       query = "SELECT * FROM operacao WHERE operacao LIKE '%" + where + "%';";
                    }
                    else 
                    {
                       query = "SELECT * FROM operacao;";
                    }

                    using (command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (lerDados = command.ExecuteReader())
                        {
                            while (lerDados.Read())
                            {
                                operacao = new Operacoes();

                                operacao.id = lerDados.GetInt32(0);
                                operacao.Operacao = lerDados.GetString(2);
                                operacao.Custo = lerDados.GetDecimal(3);
                                operacao.Observacoes = lerDados.GetString(4);

                                listaProcessos.Add(operacao);
                            }
                        }
                    }
                }

                GridOperacoes.ItemsSource = listaProcessos;
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        private void CB_Cliente_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                listaCliente = new List<Cliente>();

                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = "SELECT id, cliente FROM cliente;";

                    using (command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (lerDados = command.ExecuteReader())
                        {
                            while (lerDados.Read())
                            {
                                Cliente cli = new Cliente();

                                cli.id = lerDados.GetInt32(0);
                                cli.Cli = lerDados.GetString(1);

                                listaCliente.Add(cli);                                
                            }
                        }
                    }

                    CB_Cliente.ItemsSource = listaCliente;
                }
                    
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        private void CB_FASE_Loaded(object sender, RoutedEventArgs e)
        {
            CB_FASE.Items.Add("CORTE");
            CB_FASE.Items.Add("COSTURA");
            CB_FASE.Items.Add("ACABAMENTO");
            CB_FASE.Items.Add("DESENVOLVIMENTO");
            CB_FASE.Items.Add("LAVANDERIA");
            CB_FASE.Items.Add("LINHA");
        }

        private void GridOperacoes_SelectionChanged_1(object sender, SelectionChangedEventArgs e) //Envia os dados da linha selecionada da GridOperações para as textBox para montar o custo 
        {
            if (GridOperacoes.SelectedItem != null)
            {

                var operacao = (Operacoes)GridOperacoes.SelectedItem;

                txtId.Text = operacao.id.ToString();
                txtCusto.Text = operacao.Custo.ToString();
                txtOperacao.Text = operacao.Operacao;
            }
        }

        private void btnIncluir_Click(object sender, RoutedEventArgs e) //inclui os dados na grid montar
        {
            if (string.IsNullOrWhiteSpace(txtFreq.Text) || string.IsNullOrWhiteSpace(CB_FASE.Text))
            {
                MessageBox.Show("Você não informou a fase ou a frequência");
            }
            else
            {
                //listaProcessos = new List<Operacoes>();

                operacao = new Operacoes();

                Decimal custo = Decimal.Parse(txtCusto.Text) * Int32.Parse(txtFreq.Text);

                operacao.id = Int32.Parse(txtId.Text);
                operacao.Operacao = txtOperacao.Text;
                operacao.Fase = CB_FASE.Text;
                operacao.Custo = custo;
                operacao.Freq = Int32.Parse(txtFreq.Text);

                listaOperacoesSelecionadas.Add(operacao);

                CollectionViewSource.GetDefaultView(GridMontar.ItemsSource).Refresh();

                Soma(CB_FASE.Text, custo);
            }            
        }

        private void Soma(String fase, Decimal custo)
        {
            if (fase == "CORTE")
            {
                txtTotal.Text = (custo + Decimal.Parse(txtTotal.Text)).ToString();
            }
            else
            {
                txtTotalCostura.Text = (custo + Decimal.Parse(txtTotalCostura.Text)).ToString();
                txtTotal.Text = (custo + Decimal.Parse(txtTotal.Text)).ToString();
            }
        }//Soma o total da costura e o total da costura com o corte

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            int num = GridMontar.Items.Count; //contar o total de linhas

            try
            {
                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = @"INSERT INTO modelo (data_criacao, referencia, colecao, id_cliente, descricao, preco)
                              VALUES (@DATA, @REF, @COL, @CLI, @DESC, @PRECO) SELECT SCOPE_IDENTITY();";

                    int codCli = Convert.ToInt32(CB_Cliente.SelectedValue);
                    string dia = DateTime.Now.ToString("dd/MM/yyyy");
                    string preco = txtTotal.Text;                    

                    command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DATA", dia);
                    command.Parameters.AddWithValue("@COL", txtCol.Text);
                    command.Parameters.AddWithValue("@REF", txtRef.Text);
                    command.Parameters.AddWithValue("@CLI", codCli);
                    command.Parameters.AddWithValue("@DESC", txtDesc.Text);
                    command.Parameters.AddWithValue("@PRECO", Decimal.Parse(preco));

                    connection.Open();

                    Int32 oc = Convert.ToInt32(command.ExecuteScalar());

                    query = @"INSERT INTO montar_custo (oc, id_operacao, freq, fase, total)
                              VALUES (@OC, @ID_OPERACAO, @FREQ, @FASE, @TOTAL);";

                    foreach (var operacao in listaOperacoesSelecionadas)
                    {
                        command = new SqlCommand(query, connection);

                        command.Parameters.AddWithValue("@oc", oc);
                        command.Parameters.AddWithValue("@ID_OPERACAO", operacao.id);
                        command.Parameters.AddWithValue("@FREQ", operacao.Freq);
                        command.Parameters.AddWithValue("@FASE", operacao.Fase);
                        command.Parameters.AddWithValue("@TOTAL", operacao.Custo);

                        command.ExecuteNonQuery();
                    }
                    
                }
                MessageBox.Show("Custo criado com sucesso!");
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
                this.Close();
     
            }
            
        }//insere os dados da tabela modelo  e montar custo

        private void MenuItemExcluir_Click(object sender, RoutedEventArgs e)//Excluir uma linha selecionada da Grid Montar
        {
            var operacao = (Operacoes)GridMontar.SelectedItem;

            string fase = operacao.Fase;
            decimal custo = operacao.Custo;

            if (fase == "CORTE")
            {
                txtTotal.Text = (Decimal.Parse(txtTotal.Text) - custo).ToString();
            }
            else
            {
                txtTotalCostura.Text = (Decimal.Parse(txtTotalCostura.Text) - custo).ToString();
                txtTotal.Text = (Decimal.Parse(txtTotal.Text) - custo).ToString();
            }

            listaOperacoesSelecionadas.RemoveAt(GridMontar.SelectedIndex);
            CollectionViewSource.GetDefaultView(GridMontar.ItemsSource).Refresh();
        }

        private void txtBusca_TextChanged(object sender, TextChangedEventArgs e)
        {
            readFromDatabase(txtBusca.Text);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Sessao.JanelaMontar = false;
        }
    }
}
