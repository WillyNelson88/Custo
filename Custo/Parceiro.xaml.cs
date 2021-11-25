using System;
using System.Collections.Generic;
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
    /// Lógica interna para Parceiroxaml.xaml
    /// </summary>
    public partial class Parceiro : Window
    {
        private List<Cliente> listaCliente;
        private Banco dataBase;

        public Parceiro()
        {
            InitializeComponent();
            ReadFromDatabase();
        }

        //Ler dados da tabela cliente e exibir na grid
        private void ReadFromDatabase()
        {
            txtCli.Clear();
            txtRazao.Clear();
            txtEndereco.Clear();

            dataBase = new Banco();
            
            listaCliente = new List<Cliente>();

            listaCliente = dataBase.GetCliente();
        
           DataGridCliente.ItemsSource = listaCliente;           
        }

        //Inserir novo cliente
        private void BtnIncluir_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCli.Text) || string.IsNullOrWhiteSpace(txtRazao.Text) || string.IsNullOrWhiteSpace(txtEndereco.Text))
            {
                MessageBox.Show("Não é possível inserir dados vazios");
            }
            else
            {
                dataBase = new Banco();

                dataBase.InsertCliente(txtCli.Text, txtRazao.Text, txtEndereco.Text);

                ReadFromDatabase();
            }            
        }

        //Pegar dados da linha selecionada no dataGrid
        private void DataGridCliente_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (DataGridCliente.SelectedItem != null)
            {

                var cliente = (Cliente)DataGridCliente.SelectedItem;

                txtCli.Text = cliente.Cli;
                txtRazao.Text = cliente.Razao;
                txtEndereco.Text = cliente.Endereco;
            }
        }

        //Excluir um cliente
        private void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridCliente.SelectedItem != null)
            {
                dataBase = new Banco();
                var cliente = (Cliente)DataGridCliente.SelectedItem;
            
                dataBase.ExcluiCliente(cliente.Id);

                ReadFromDatabase();
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridCliente.SelectedItem != null)
            {
                var cliente = (Cliente)DataGridCliente.SelectedItem;
                dataBase = new Banco();

                dataBase.EditarCliente(txtCli.Text, txtRazao.Text, txtEndereco.Text, cliente.Id);

                ReadFromDatabase();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Sessao.JanelaParceiro = false;
        }
    }

}
