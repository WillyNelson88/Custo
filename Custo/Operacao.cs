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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Custo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Operacao : Window
    {

        //ADO.NET
        public SqlConnection connection; //Conexão com o sql server
        public SqlCommand command;       // Execução de comando no sql server
        public SqlDataReader dataReader; //Lê os resultados de um comando do sql executado no sql server

        public string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=custo;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public string query = string.Empty; //Usado para criar os comandos no sql


        private List<Operacoes> listaOperacoes;
        private Operacoes operacao;

        public Operacao()
        {
            InitializeComponent();
            readFromDatabase();
        }

        //Ler dados da tabela e exibir na grid
        private void readFromDatabase()
        {
            try
            {
                txtOperacao.Clear();
                txtCusto.Clear();
                txtObservacoes.Clear();

                listaOperacoes = new List<Operacoes>();

                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = "SELECT * FROM operacao;";

                    using (command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                operacao = new Operacoes();

                                operacao.id = dataReader.GetInt32(0);
                                operacao.Data = dataReader.GetString(1);
                                operacao.Operacao = dataReader.GetString(2);
                                operacao.Custo = dataReader.GetDecimal(3);
                                operacao.Observacoes = dataReader.GetString(4);

                                listaOperacoes.Add(operacao);
                            }
                        }
                    }
                }
                DataGridOperacoes.ItemsSource = listaOperacoes;
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
                listaOperacoes = null;
            }
        }

        //Inserir operações na tabela operações
        private void btnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOperacao.Text) || string.IsNullOrWhiteSpace(txtCusto.Text) || string.IsNullOrWhiteSpace(txtObservacoes.Text))
            {
                MessageBox.Show("Não é possível inserir dados vazios");
            }
            else
            {
                try
                {
                    using (connection = new SqlConnection())
                    {
                        connection.ConnectionString = connectionString;

                        query = "INSERT INTO OPERACAO (DATA_CRIACAO, OPERACAO, CUSTO, OBSERVACOES)  VALUES (@DATA, @OPERACAO, @CUSTO, @OBS)";

                        string data = DateTime.Now.ToString("dd/MM/yyyy");
                        string preco = txtCusto.Text;
                        preco = preco.Replace(',', '.');

                        command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@DATA", data);
                        command.Parameters.AddWithValue("@OPERACAO", txtOperacao.Text);
                        command.Parameters.AddWithValue("@CUSTO", preco);
                        command.Parameters.AddWithValue("@OBS", txtObservacoes.Text);

                        connection.Open();

                        command.ExecuteNonQuery();

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
                    readFromDatabase();
                }
            }
        }

        //Pegar dados da linha selecionada no dataGrid
        private void DataGridOperacoes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (DataGridOperacoes.SelectedItem != null)
            {

                var operacao = (Operacoes)DataGridOperacoes.SelectedItem;

                txtOperacao.Text = operacao.Operacao;
                txtCusto.Text = operacao.Custo.ToString();
                txtObservacoes.Text = operacao.Observacoes;
            }
        }

        //Excluir dados da linha selecionada
        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    if (DataGridOperacoes.SelectedItem != null)
                    {
                        var operacao = (Operacoes)DataGridOperacoes.SelectedItem;
                        string id = operacao.id.ToString();

                        if (MessageBox.Show("Tem certeza que desejas excluir a operação?", "Excluir operação", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            connection.ConnectionString = connectionString;
                            query = "DELETE FROM OPERACAO WHERE id = @ID";

                            command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@ID", id);

                            connection.Open();

                            command.ExecuteNonQuery();
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

                readFromDatabase();
            }
        }

        //Editar dados da linha selecionada
        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    if (DataGridOperacoes.SelectedItem != null)
                    {
                        var operacao = (Operacoes)DataGridOperacoes.SelectedItem;

                        connection.ConnectionString = connectionString;
                        query = "UPDATE OPERACAO SET operacao = @OPERACAO, custo = @CUSTO, observacoes = @OBSERVACOES WHERE id = @ID";

                        command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@OPERACAO", txtOperacao.Text);
                        command.Parameters.AddWithValue("@CUSTO", Decimal.Parse(txtCusto.Text));
                        command.Parameters.AddWithValue("@OBSERVACOES", txtObservacoes.Text);
                        command.Parameters.AddWithValue("@ID", operacao.id);

                        connection.Open();
                        command.ExecuteNonQuery();

                        MessageBox.Show("Operação editada com sucesso!");
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
                readFromDatabase();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Sessao.JanelaCusto = false;

        }
    }
}