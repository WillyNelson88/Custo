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
    public partial class Cobranca : Window
    {
        private SqlConnection connection; //Conexão com o sql server
        private SqlCommand command;       // Execução de comando no sql server
        private SqlDataReader lerDados; //Lê os resultados de um comando do sql executado no sql server

        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=custo;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private string query = string.Empty; //Usado para criar os comandos no sql

        private Modelos modelo;
        private List<Modelos> listaModelos;

        public Cobranca()
        {
            InitializeComponent();
            readFromDatabase(null, null);
        }

        private void readFromDatabase(string col, string where)
        {
            try
            {

                listaModelos = new List<Modelos>();

                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    if ((where != null) && (col != null))
                    {
                        query = @"SELECT mo.oc, co.talao, cli.cliente, mo.colecao, mo.referencia, mo.descricao, co.preco " +
                                "FROM modelo mo, cliente cli, cobranca co " +
                                "WHERE co.oc = mo.oc " +
                                "AND cli.id = mo.id_cliente " +
                                "AND " + col + " LIKE '%" + where + "%'";
                    }
                    else
                    {
                        query = @"SELECT mo.oc, co.talao, cli.cliente, mo.colecao, mo.referencia, mo.descricao, co.preco " +
                                "FROM modelo mo, cliente cli, cobranca co " +
                                "WHERE co.oc = mo.oc " +
                                "AND cli.id = mo.id_cliente ";
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
                                modelo.talao = lerDados.GetInt32(1);
                                modelo.cliente = lerDados.GetString(2);
                                modelo.colecao = lerDados.GetString(3);
                                modelo.referencia = lerDados.GetString(4);                                
                                modelo.descricao = lerDados.GetString(5);
                                modelo.preco = lerDados.GetDecimal(6);

                                listaModelos.Add(modelo);
                            }
                        }
                    }
                }

                DataGridCobranca.ItemsSource = listaModelos;
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        private void cbBusca_Loaded(object sender, RoutedEventArgs e)
        {
            cbBusca.Items.Add("mo.oc");
            cbBusca.Items.Add("co.talao");
            cbBusca.Items.Add("cli.cliente");
            cbBusca.Items.Add("mo.colecao");
            cbBusca.Items.Add("mo.referencia");
            cbBusca.Items.Add("mo.descricao");
            cbBusca.Items.Add("co.preco");
        }  

        private void txtBusca_TextChanged(object sender, TextChangedEventArgs e)
        {
            string col;

            if (string.IsNullOrWhiteSpace(cbBusca.Text))
            {
                col = "mo.referencia";
            }
            else
            {
                col = cbBusca.Text;           
            }

            readFromDatabase(col, txtBusca.Text);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Sessao.JanelaCobrar = false;
        }
    }
}
