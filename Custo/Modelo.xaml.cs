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
    /// Lógica interna para Modelo.xaml
    /// </summary>
    public partial class Modelo : Window
    {
        private SqlConnection connection; //Conexão com o sql server
        private SqlCommand command;       // Execução de comando no sql server
        private SqlDataReader lerDados; //Lê os resultados de um comando do sql executado no sql server

        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=custo;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private string query = string.Empty; //Usado para criar os comandos no sql

        private List<Modelos> listaModelos;
        private Modelos modelo;

        public Modelo()
        {
            InitializeComponent();
            readFromDatabase(null);
        }

        private void readFromDatabase(string where)
        {
            try
            {

                listaModelos = new List<Modelos>();

                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;
                   
                    if (where != null)
                    {
                        query = @"SELECT mo.oc, cli.cliente, mo.referencia, mo.colecao, mo.descricao, mo.preco " +
                                "FROM modelo mo, cliente cli " +
                                "WHERE cli.id = mo.id_cliente " +
                                "AND mo.referencia LIKE '%" + where + "%' " +
                                "OR cli.cliente LIKE '%" + where + "%';";
                    }
                    else
                    {
                        query = "SELECT mo.oc, cli.cliente, mo.referencia, mo.colecao, mo.descricao, mo.preco FROM modelo mo, cliente cli WHERE cli.id = mo.id_cliente;";
                    }

                    using (command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (lerDados = command.ExecuteReader())
                        {
                            while (lerDados.Read())
                            {
                                modelo = new Modelos();

                                modelo.oc = lerDados.GetInt32(0);
                                modelo.cliente = lerDados.GetString(1);
                                modelo.referencia = lerDados.GetString(2);
                                modelo.colecao = lerDados.GetString(3);
                                modelo.descricao = lerDados.GetString(4);
                                modelo.preco = lerDados.GetDecimal(5);

                                listaModelos.Add(modelo);
                            }
                        }
                    }
                }

                DataGridModelos.ItemsSource = listaModelos;
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        private void btnDetalhe_Click(object sender, RoutedEventArgs e)
        {
            var modelo = (Modelos)DataGridModelos.SelectedItem;

            int oc = modelo.oc;

            var detail = new Detahes(oc);
            detail.Show();      

        }

        private void txtBuscaModelo_TextChanged(object sender, TextChangedEventArgs e)
        {
            readFromDatabase(txtBuscaModelo.Text);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Sessao.JanelaModelo = false;
        }
    }    
}
