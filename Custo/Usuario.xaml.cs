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
    /// Lógica interna para Usuario.xaml
    /// </summary>
    public partial class Usuario : Window
    {
        private List<Login> listaUsuario;
        private Banco dataBase;

        public Usuario()
        {
            InitializeComponent();
            ReadFromDatabase();
        }

        //Listar os usuários;
        private void ReadFromDatabase()
        {
            txtSenha.Clear();
            txtUsuario.Clear();
            CbSituacao.SelectedIndex = -1;

            dataBase = new();
            listaUsuario = new List<Login>();

            listaUsuario = dataBase.GetUsuario();
            
            DataGridUsuarios.ItemsSource = listaUsuario;            
        }

        private void CbSituacao_Loaded(object sender, RoutedEventArgs e)
        {
            CbSituacao.Items.Add("USER");
            CbSituacao.Items.Add("ADMIN");
        }

        //Adiciona um usuário na tabela login;
        private void BtnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtSenha.Text) || string.IsNullOrWhiteSpace(CbSituacao.Text))
            {
                MessageBox.Show("Não é possível inserir dados vazios");
            }
            else
            {
                dataBase = new();
                dataBase.AdicionaUsuario(txtUsuario.Text, txtSenha.Text, CbSituacao.Text);
            }

            ReadFromDatabase();
        }

        //Pegar dados da linha selecionada no dataGrid
        private void DataGridUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (DataGridUsuarios.SelectedItem != null)
            {
                var login = (Login)DataGridUsuarios.SelectedItem;

                txtUsuario.Text = login.usuario;
                CbSituacao.SelectedItem = login.situacao;
            }
        }

        //Excluir dados da linha selecionada
        private void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridUsuarios.SelectedItem != null)
            {
                var login = (Login)DataGridUsuarios.SelectedItem;
                int id = login.id;

                dataBase = new();

                dataBase.ExcluiUsuario(id);
            }            
            ReadFromDatabase();
        }        

        //Editar dados da linha selecionada
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSenha.Text) || string.IsNullOrWhiteSpace(CbSituacao.Text) || string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                MessageBox.Show("Não é possível incluir valores vazios");
            }
            else
            {
                if (DataGridUsuarios.SelectedItem != null)
                {

                    var login = (Login)DataGridUsuarios.SelectedItem;

                    dataBase = new();

                    dataBase.EditarUsuario(txtUsuario.Text, txtSenha.Text, CbSituacao.Text, login.id);
                }
                
                
            }

            ReadFromDatabase();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Sessao.JanelaUsuario = false;
        }
    }
}
