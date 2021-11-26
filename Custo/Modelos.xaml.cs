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
    /// Lógica interna para Modelo.xaml
    /// </summary>
    public partial class Modelos : Window
    {
       
        private List<Modelo> listaModelos;
        private Banco dataBase;

        public Modelos()
        {
            InitializeComponent();
            ReadFromDatabase(null);
        }

        //Busca os modelos na tabela modelos
        private void ReadFromDatabase(string where)
        {
            dataBase = new();

            listaModelos = new List<Modelo>();

            listaModelos = dataBase.GetModelos(where);

            DataGridModelos.ItemsSource = listaModelos;            
        }

        //Abre a janela de detahes gerar pdf e montar cobraça
        private void BtnDetalhe_Click(object sender, RoutedEventArgs e)
        {
            var modelo = (Modelo)DataGridModelos.SelectedItem;

            int oc = modelo.Oc;

            var detail = new Detahes(oc);
            detail.Show();      
        }

        //Fazer a busca
        private void TxtBuscaModelo_TextChanged(object sender, TextChangedEventArgs e)
        {
            ReadFromDatabase(TxtBuscaModelo.Text);
        }

        //Fecha a sessão
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Sessao.JanelaModelo = false;
        }
    }    
}
