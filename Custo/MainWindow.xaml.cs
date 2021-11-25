using custo;
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
    public partial class MainWindow : Window
    {
        private Banco dataBase;
        
        public MainWindow()
        {
            InitializeComponent();
        }            

        //Método para buscar os usuários do banco e validar o usuário que está tentando logar;
        private void BtnEntrar_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUser.Text;
            string senha = txtPass.Password;

            dataBase = new Banco();

            var login = dataBase.Autenticar(usuario);
            
            //Se não retornar a situação o usuário não existe na base; 
            if (login.situacao == null)
            {
                MessageBox.Show("Usuário não encontrado");                
            }            
            else //Se retornar valida a senha digitada; 
            {
                string verify = login.senha;

                if (BCrypt.Net.BCrypt.Verify(senha, verify))
                {
                    Sessao.SessaoUsuario = login.usuario;
                    Sessao.SessaoSituacao = login.situacao;

                    Main principal = new Main();
                    principal.Show();
                }
                else
                {
                    MessageBox.Show("Senha incorreta"); //Se a senha estiver incorreta não permite o acesso
                }
            }
            
            this.Close();
        }
    }
}
