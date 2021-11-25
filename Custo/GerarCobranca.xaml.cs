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
    /// Lógica interna para Detalhes.xaml
    /// </summary>
    public partial class GerarCobranca : Window
    {
        private List<Processo> listaProcessos;
        private List<Processo> listaFase;

        private Banco dataBase;
        private Modelo modelo;
        private Processo processo;

        public GerarCobranca()
        {
            InitializeComponent();           
        }

        public GerarCobranca(int oc) : this()
        {
            Detalhes(oc);
            listaFase = new List<Processo>();
            DataGridGerarCobranca.ItemsSource = listaFase;
        }

        //Pega os dados do modelo referente a oc;\
        private void Detalhes(int oc)
        {
            dataBase = new();
            modelo = new();

            modelo = dataBase.Detalhes(oc);

            txtOC.Text = modelo.Oc.ToString();
            txtDataCriacao.Text = modelo.Data_criacao;
            txtReferencia.Text = modelo.Referencia;
            txtColecao.Text = modelo.Colecao;
            txtCliente.Text = modelo.Cliente;
            txtDescricao.Text = modelo.Descricao;

            Operacoes(oc);
        }

        //Pega as operações do modelo referente a oc e agrupa por fase;
        public void Operacoes(int oc)
        {
            dataBase = new();
            listaProcessos = new();

            listaProcessos = dataBase.GetDetalhes(oc);
            DataGridOperacoesCobranca.ItemsSource = listaProcessos;           
        }

        //Preenche as text box de montar os custos;
        private void DataGridOperacoesCobranca_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridOperacoesCobranca.SelectedItem != null)
            {
                var operacao = (Processo)DataGridOperacoesCobranca.SelectedItem;

                txtProcesso.Text = operacao.Fase.ToString();
                txtPreco.Text = operacao.Custo.ToString();
                txtPercentual.Text = null;
            }
        }

        //Inclui a operação da text Box na Grid Montar Custo
        private void BtnIncluir_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProcesso.Text) || string.IsNullOrWhiteSpace(txtPreco.Text))
            {
                MessageBox.Show("Não é possível incluir valores vazios");
            }
            else
            {
                processo = new Processo();

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

                processo.Fase = txtProcesso.Text;
                processo.Custo = preco;
                processo.Percentual = percentual;
                processo.Total = total;

                listaFase.Add(processo);

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

        private void CbTipo_Loaded(object sender, RoutedEventArgs e)
        {
            CbTipo.Items.Add("PRODUÇÃO");
            CbTipo.Items.Add("MOSTRUÁRIO");
            CbTipo.Items.Add("PILOTO");
        }

        private void BtnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTalao.Text) || string.IsNullOrWhiteSpace(txtTotal.Text) || string.IsNullOrWhiteSpace(CbTipo.Text))
            {
                MessageBox.Show("Não é possível finalizar, favor verificar os dados.");
            }
            else
            {
                int talao = Int32.Parse(txtTalao.Text);
                int oc = Int32.Parse(txtOC.Text);
                decimal preco = Decimal.Parse(txtTotal.Text);    

                dataBase = new();

                dataBase.IncluiCobranca(talao, oc, CbTipo.Text, preco);

                MessageBox.Show("Cobrança finalizada com sucesso");                
            }

            this.Close();
        }
    }
}
