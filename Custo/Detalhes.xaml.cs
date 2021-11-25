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
        private Modelo modelo;
        private List<Processo> listaProcessos;
        private Banco dataBase;

        public Detahes()
        {
            InitializeComponent();
        }

        public Detahes(int oc) : this()
        {
            Detalhar(oc);
        }

        private void Detalhar(int oc)
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

        public void Operacoes(int oc)
        {
            dataBase = new();
            
            listaProcessos = new List<Processo>();
            listaProcessos = dataBase.GetOperacaoModelo(oc);
            DataGridDetalhes.ItemsSource = listaProcessos;           
        }

        private void BtnCobranca_Click(object sender, RoutedEventArgs e)
        {
            int oc = modelo.Oc;

            var cobranca = new GerarCobranca(oc);

            cobranca.Show();
        }

        private void BtnGerarPDF_Click(object sender, RoutedEventArgs e)
        {
            int oc = modelo.Oc;
            GerarPDF pdf = new GerarPDF(oc);
        }
    }
}
