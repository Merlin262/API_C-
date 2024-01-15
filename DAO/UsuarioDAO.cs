using projeto.Models;

namespace wstesteFull.DAO
{
    public class UsuarioDAO
    {
        //CRUD no banco de dados de pessoas

        public bool Inserir(Usuario usuario)
        {
            string query = "insert into usuario values(,,,,)";

            return true;
        }
    }
}