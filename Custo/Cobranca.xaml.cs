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
    public partial class Cobranca : Window
    {
        private Banco dataBase;
        private List<Modelo> listaModelos;

        public Cobranca()
        {
            InitializeComponent();
            ReadFromDatabase(null, null);
        }

        //Carrega o dataGridCobranca;
        private void ReadFromDatabase(string col, string where)
        {
            dataBase = new();
            listaModelos = new List<Modelo>();

            listaModelos = dataBase.GetCobranca(col, where);

            DataGridCobranca.ItemsSource = listaModelos;          
        }

        //Carrega a combo box de busca;
        private void CbBusca_Loaded(object sender, RoutedEventArgs e)
        {
            CbBusca.Items.Add("mo.oc");
            CbBusca.Items.Add("co.talao");
            CbBusca.Items.Add("cli.cliente");
            CbBusca.Items.Add("mo.colecao");
            CbBusca.Items.Add("mo.referencia");
            CbBusca.Items.Add("mo.descricao");
            CbBusca.Items.Add("co.preco");
        }  

        //Faz a busca;
        private void TxtBusca_TextChanged(object sender, TextChangedEventArgs e)
        {
            string col;

            if (string.IsNullOrWhiteSpace(CbBusca.Text))
            {
                col = "mo.referencia";
            }
            else
            {
                col = CbBusca.Text;           
            }

            ReadFromDatabase(col, TxtBusca.Text);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Sessao.JanelaCobrar = false;
        }
    }
}
