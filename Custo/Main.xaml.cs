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
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Sessao.JanelaMontar == true)
            {
                MessageBox.Show("Esse recurso já está sendo usado!");
            }
            else
            {
                Montar montarCusto = new Montar();
                montarCusto.Show();

                Sessao.JanelaMontar = true;
            }            
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (Sessao.JanelaCusto == true)
            {
                MessageBox.Show("Esse recurso já está sendo usado!");
            }
            else 
            {
                Operacao operacao = new Operacao();
                operacao.Show();
                Sessao.JanelaCusto = true;
            }           
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (Sessao.JanelaParceiro == true)
            {
                MessageBox.Show("Esse recurso já está sendo usado!");
            }
            else
            {
                Parceiro parceiro = new Parceiro();
                parceiro.Show();

                Sessao.JanelaParceiro = true;
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            if (Sessao.JanelaModelo == true)
            {
                MessageBox.Show("Esse recurso já está sendo usado!");
            }
            else
            {
                Modelo model = new Modelo();
                model.Show();

                Sessao.JanelaModelo = true;
            }            
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            if (Sessao.JanelaCobrar == true)
            {
                MessageBox.Show("Esse recurso já está sendo usado!");
            }
            else
            {
                Cobranca cobrar = new Cobranca();
                cobrar.Show();

                Sessao.JanelaCobrar = true;
            }            
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            if (Sessao.JanelaUsuario == true)
            {
                MessageBox.Show("Esse recurso já está sendo usado!");
            }
            else
            {
                if (Sessao.SessaoSituacao == "USER")
                {
                    MessageBox.Show("Você não tem permissão para acessar este recurso!", "Acesso negado", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    Usuario user = new Usuario();
                    user.Show();

                    Sessao.JanelaUsuario = true;
                }                
            }                       
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}
