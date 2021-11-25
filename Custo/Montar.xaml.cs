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
    /// Lógica interna para Montar.xaml
    /// </summary>
    public partial class Montar : Window
    {       
        private List<Processo> listaProcessos;
        private List<Cliente> listaCliente;
        private Processo operacao;
        private List<Processo> listaOperacoesSelecionadas;
        private Banco dataBase;

        public Montar()
        {
            InitializeComponent();
            ReadFromDatabase(null);

            listaOperacoesSelecionadas = new List<Processo>();
            GridMontar.ItemsSource = listaOperacoesSelecionadas;
        }

        //Ler dados da tabela e exibir na grid;
        private void ReadFromDatabase(string where)
        {
            dataBase = new Banco();

            listaProcessos = new List<Processo>();

            listaProcessos = dataBase.GetOperacoes(where);

            GridOperacoes.ItemsSource = listaProcessos;       
        }

        //Carregar a combo box cliente;
        private void CB_Cliente_Loaded(object sender, RoutedEventArgs e)
        {
            dataBase = new();

            listaCliente = new();

            listaCliente = dataBase.CbCliente();

            CB_Cliente.ItemsSource = listaCliente;
        }

        //Carregar a combo box fase;
        private void CB_FASE_Loaded(object sender, RoutedEventArgs e)
        {
            CB_FASE.Items.Add("CORTE");
            CB_FASE.Items.Add("COSTURA");
            CB_FASE.Items.Add("ACABAMENTO");
            CB_FASE.Items.Add("DESENVOLVIMENTO");
            CB_FASE.Items.Add("LAVANDERIA");
            CB_FASE.Items.Add("LINHA");
        }

        //Seleciona os dados da grid de operações para as text box de montar os custos;
        private void GridOperacoes_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (GridOperacoes.SelectedItem != null)
            {
                var operacao = (Processo)GridOperacoes.SelectedItem;

                txtId.Text = operacao.Id.ToString();
                txtCusto.Text = operacao.Custo.ToString();
                txtOperacao.Text = operacao.Operacao;
            }
        }

        //Inclui os dados das text box na grid de de montar os custos;
        private void BtnIncluir_Click(object sender, RoutedEventArgs e) //inclui os dados na grid montar
        {
            if (string.IsNullOrWhiteSpace(txtFreq.Text) || string.IsNullOrWhiteSpace(CB_FASE.Text))
            {
                MessageBox.Show("Você não informou a fase ou a frequência");
            }
            else
            {
                operacao = new Processo();

                Decimal custo = Decimal.Parse(txtCusto.Text) * Int32.Parse(txtFreq.Text);

                operacao.Id = Int32.Parse(txtId.Text);
                operacao.Operacao = txtOperacao.Text;
                operacao.Fase = CB_FASE.Text;
                operacao.Custo = custo;
                operacao.Freq = Int32.Parse(txtFreq.Text);

                listaOperacoesSelecionadas.Add(operacao);

                CollectionViewSource.GetDefaultView(GridMontar.ItemsSource).Refresh();

                Soma(CB_FASE.Text, custo);

                txtId.Clear();
                txtOperacao.Clear();
                txtFreq.Text = "1";
                CB_FASE.SelectedIndex = -1;
                txtCusto.Clear();
            }            
        }

        //Método para somar as operações de costura separada das operações das outras fases;
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

        //Salva os dados nas tabelas modelo e montar_custo;
        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            int codCli = Convert.ToInt32(CB_Cliente.SelectedValue); 
            decimal preco = Decimal.Parse(txtTotal.Text);

            dataBase = new();

            int oc = dataBase.CriarModelo(txtRef.Text, txtCol.Text, codCli, txtDesc.Text, preco);

            foreach (var operacao in listaOperacoesSelecionadas)
            {
                dataBase.MontaPreco(oc, operacao.Id, operacao.Freq, operacao.Fase, operacao.Custo);
            }

            MessageBox.Show("Custo criado com sucesso!");

            this.Close(); //Fechando a aplicação porque não consegui limpar os dados da dataGrid;*/
        }

        //Exclui uma linha da tabela de montar os custos;
        private void MenuItemExcluir_Click(object sender, RoutedEventArgs e)//Excluir uma linha selecionada da Grid Montar
        {
            var operacao = (Processo)GridMontar.SelectedItem;

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

        //Buscar os dados com condição;
        private void TxtBusca_TextChanged(object sender, TextChangedEventArgs e)
        {
            ReadFromDatabase(TxtBusca.Text);
        }

        //Validar se a janela está aberta ou não;
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Sessao.JanelaMontar = false;
        }
    }
}