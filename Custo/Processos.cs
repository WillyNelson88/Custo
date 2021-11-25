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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Custo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Processos : Window
    {
        private List<Processo> listaProcessos;
        private Banco dataBase;
        
        public Processos()
        {
            InitializeComponent();
            ReadFromDatabase();
        }

        //Ler dados da tabela operacao e exibir na grid
        private void ReadFromDatabase()
        {
            string where = null;

            txtOperacao.Clear();
            txtCusto.Clear();
            txtObservacoes.Clear();

            dataBase = new();

            listaProcessos = new List<Processo>();

            listaProcessos = dataBase.GetOperacoes(where);

            DataGridProcessos.ItemsSource = listaProcessos;
        }

        //Inserir operações na tabela operações
        private void BtnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOperacao.Text) || string.IsNullOrWhiteSpace(txtCusto.Text) || string.IsNullOrWhiteSpace(txtObservacoes.Text))
            {
                MessageBox.Show("Não é possível inserir dados vazios");
            }
            else
            {
                string operacao = txtOperacao.Text;
                string custo = txtCusto.Text;
                string obs = txtObservacoes.Text;

                dataBase = new Banco();

                dataBase.InsertOperacao(operacao, custo, obs);

                ReadFromDatabase();
            }
        }
        

        //Pegar dados da linha selecionada no dataGrid
        private void DataGridProcessos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (DataGridProcessos.SelectedItem != null)
            {

                var operacao = (Processo)DataGridProcessos.SelectedItem;

                txtOperacao.Text = operacao.Operacao;
                txtCusto.Text = operacao.Custo.ToString();
                txtObservacoes.Text = operacao.Observacoes;
            }
        }

        //Excluir dados da linha selecionada
        private void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridProcessos.SelectedItem != null)
            {
                var processo = (Processo)DataGridProcessos.SelectedItem;
                string id = processo.Id.ToString();

                dataBase = new Banco();

                dataBase.ExcluirOperacao(id);

                ReadFromDatabase();
            }
        }

        //Editar dados da linha selecionada
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridProcessos.SelectedItem != null)
            {
                var operacao = (Processo)DataGridProcessos.SelectedItem;

                string processo = txtOperacao.Text;
                string observacoes = txtObservacoes.Text;
                int id = operacao.Id;
                string preco = txtCusto.Text;
                decimal custo = Decimal.Parse(preco);

                dataBase = new Banco();

                dataBase.EditarOperacao(processo, custo, observacoes, id);

                ReadFromDatabase();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Sessao.JanelaCusto = false;

        }
    }
}