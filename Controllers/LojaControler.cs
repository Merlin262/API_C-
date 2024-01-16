using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using projeto.Models;

namespace projeto.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LojaControler : ControllerBase
    {
        static private List<Usuario> usuarios;
        static private List<Pedido> pedidos;
        static private List<Itens> itensLoja;
        
        public LojaControler()
        {
            if( usuarios == null )
            {
                usuarios = new List<Usuario>();
                pedidos = new List<Pedido>();
                itensLoja = new List<Itens>();
                CargaTestes();
            }
        }

        void CargaTestes()
        {
            //populando o sistema para testar o CRUD
            List<Pedido> pedidosTeste1 = new List<Pedido>();
            List<Pedido> pedidosTeste2 = new List<Pedido>();
            Usuario joao = new Usuario("João", "12345678900", pedidosTeste1);
            Usuario merlin = new Usuario("Merlin","09876543211", pedidosTeste2);

            Itens teclado = new Itens("teclado", 20);
            Itens mouse = new Itens("mouse", 30);
            Itens monitor = new Itens("monitor", 10);

            Pedido pedido1 = new Pedido("01", monitor, 3);
            Pedido pedido2 = new Pedido("02", teclado, 5);
            Pedido pedido3 = new Pedido("03", mouse, 2);

            itensLoja.Add(monitor);
            itensLoja.Add(teclado);
            itensLoja.Add(mouse);
            
            pedidos.Add(pedido1);
            pedidos.Add(pedido2);
            pedidos.Add(pedido3);

            pedidosTeste1.Add(pedido3);
            pedidosTeste1.Add(pedido1);
            pedidosTeste2.Add(pedido2);
            
            usuarios.Add(joao);
            usuarios.Add(merlin);
           
        }

        //Retorna todos os Usuarios cadastrados
        [HttpGet("Usuarios/MostrarUsuarios")]
        public IActionResult getTodosUsuarios()
        {
            try
            {
                List<string> formatacaoretorno = new List<string>();
                foreach (Usuario u in usuarios)
                {
                    formatacaoretorno.Add($"O Usuario tem o nome {u.Nome}; O número de CPF é {u.CPF}");
                }
                return Ok(formatacaoretorno);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao processar a solicitação: " + ex.Message);
            }
        }

        //Retorna todos os Pedidos cadastrados
        [HttpGet("Pedido/MostrarTodosPedidos")]
        public IActionResult getTodosPedidos()
        {
            try
            {
                List<string> formatacaoPedidos = new List<string>();
                foreach (Pedido p in pedidos)
                {
                    formatacaoPedidos.Add($"O item do pedido é {p.Item.NomeItem}; O número de itens é {p.QtdItens}; e seu ID é : {p.Id}");
                }
                return Ok(formatacaoPedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao processar a solicitação: " + ex.Message);
            }
        }

        //Retorna todos os itens diponiveis para a venda
        [HttpGet("Itens/MostrarItensAVenda")]
        public IActionResult getTodosItensAVenda()
        {
            try
            {
                List<string> formatacaoretorno = new List<string>();
                foreach (Itens i in itensLoja)
                {
                    if (i.Qtd > 0)
                    {
                        formatacaoretorno.Add($"O Item tem o nome de {i.NomeItem}; E {i.Qtd} disponível para a compra");
                    }
                }
                return Ok(formatacaoretorno);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao processar a solicitação: " + ex.Message);
            }
        }
        

        //Retorna o usuarios com o CPF informado
        [HttpGet("Usuarios/MostrarUsuariosCPF/{cpfUsuarios}")]
        public IActionResult getUsuarioCPF(string cpfUsuarios)
        {
            var usuarioEncontrado = usuarios.FirstOrDefault(p => p.CPF == cpfUsuarios);

            if (usuarioEncontrado != null)
            {
                return Ok($"De acordo com o {usuarioEncontrado.CPF}, o usuario correspondente é: {usuarioEncontrado.Nome} ");
            }

            return NotFound("Pessoa não encontrada");
        }

        //Retorna todos os pedidos realizados pelo Usuario com o CPF informado
        [HttpGet("Pedido/MostrarListaDePedidosPorCPF/{cpfPedidos}")]
        public IActionResult GetListaPedidos(string cpfPedidos)
        {
            try
            {
                var usuario = usuarios.FirstOrDefault(u => u.CPF == cpfPedidos);

                if (usuario != null)
                {
                    StringBuilder pedidosInfo = new StringBuilder();
                    foreach (Pedido pedido in usuario.ListaPedidos)
                    {
                        pedidosInfo.AppendLine($"ID do Pedido: {pedido.Id}");
                        pedidosInfo.AppendLine($"Item: {pedido.Item.NomeItem}");
                        pedidosInfo.AppendLine($"Quantidade de Itens: {pedido.QtdItens}");
                        pedidosInfo.AppendLine();
                    }

                    return Ok($"A lista de pedidos do CPF {usuario.CPF} é:\n{pedidosInfo.ToString()}");
                }

                return NotFound("Pessoa não encontrada");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao processar a solicitação: " + ex.Message);
            }
        }


        [HttpGet("Pedido/MostrarItensDoPedido/{id}")]
        public IActionResult ObterItensDoPedido(string id)
        {
            try
            {
                //Procurar o pedido com base no ID
                foreach (Pedido p in pedidos)
                {
                    if (p.Id == id)
                    {
                        //Retorna os itens do pedido
                        return Ok($"O pedido de ID {p.Id} tem {p.QtdItens} unidades de {p.Item.NomeItem}");
                    }
                }

                return NotFound("Pedido não encontrado");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao processar a solicitação: " + ex.Message);
            }
        }

        
        [HttpPost("Usuario/CriarUsuario/{novoUsuario}")]
        public IActionResult InserirUsuario(Usuario novoUsuario)
        {
            try
            {
                //Validar se o CPF é válido (contém apenas dígitos ou não esta vazio)
                if (string.IsNullOrEmpty(novoUsuario.CPF) || !novoUsuario.CPF.All(char.IsDigit))
                {
                    throw new ValidacaoUsuarioException("CPF inválido.");
                }

                //Validar se o CPF tem exatamente 11 dígitos
                if (novoUsuario.CPF.Length != 11)
                {
                    throw new ValidacaoUsuarioException("CPF deve ter exatamente 11 dígitos.");
                }

                //Validar se o nome não contém números
                if (string.IsNullOrEmpty(novoUsuario.Nome))
                {
                    throw new ValidacaoUsuarioException("O campo do nome não pode estar vazio.");
                }

                if (novoUsuario.Nome.Any(char.IsDigit))
                {
                    throw new ValidacaoUsuarioException("O nome não pode conter números.");
                }

                //Verificar se o CPF do novo usuário já existe
                if (usuarios.Any(u => u.CPF == novoUsuario.CPF))
                {
                    throw new ValidacaoUsuarioException("Já existe um usuário com este CPF.");
                }

                //Adiciona os pedidos do novo usuário à lista geral de pedidos
                foreach (Pedido pedido in novoUsuario.ListaPedidos)
                {
                    //Verificar se o ID do pedido já existe
                    if (pedidos.Any(p => p.Id == pedido.Id))
                    {
                        throw new ValidacaoUsuarioException($"Já existe um pedido com o ID {pedido.Id}.");
                    }

                    var itemNaLoja = itensLoja.FirstOrDefault(item => item.NomeItem == pedido.Item.NomeItem);

                    if (itemNaLoja == null)
                    {
                        throw new ValidacaoUsuarioException($"Item '{pedido.Item.NomeItem}' não encontrado na loja");
                    }

                    //Verificar se a quantidade desejada é maior que a quantidade em estoque
                    if (pedido.QtdItens > itemNaLoja.Qtd)
                    {
                        throw new ValidacaoUsuarioException($"Quantidade desejada maior que a quantidade em estoque para o item '{itemNaLoja.NomeItem}'.");
                    }

                    //Subtrai a quantidade do estoque
                    itemNaLoja.Qtd -= pedido.QtdItens;

                    //Verifica se a quantidade no estoque é menor ou igual a zero e remove o item da lista se necessário
                    if (itemNaLoja.Qtd <= 0)
                    {
                        itensLoja.Remove(itemNaLoja);
                    }

                    //Adiciona o pedido à lista geral de pedidos
                    pedidos.Add(pedido);
                }

                //Adiciona o novo usuário à lista de usuários
                usuarios.Add(novoUsuario);

                return CreatedAtAction(nameof(InserirUsuario), new { cpf = novoUsuario.CPF }, novoUsuario);
            }
            catch (ValidacaoUsuarioException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao processar a solicitação: " + ex.Message);
            }
        }

        [HttpPost("Pedido/CriarPedido/{cpfUsuario}/{id}/{itemDesejado}/{qtdDesejada}")]
        public IActionResult InserirPedido([Required] string id, [Required] int qtdDesejada, [Required] string cpfUsuario, [Required] string itemDesejado)
        {
            try
            {
                var itemExistente = itensLoja.FirstOrDefault(n => n.NomeItem == itemDesejado);
                if (itemExistente == null)
                {
                    throw new ValidacaoPedidoException("Item não encontrado na loja");
                }

                //Verifica se o ID do pedido já foi usado
                if (pedidos.Any(pedido => pedido.Id == id))
                {
                    throw new ValidacaoPedidoException("ID do pedido já utilizado");
                }

                //Verifica se o usuário com o CPF fornecido existe
                var usuarioExistente = usuarios.FirstOrDefault(u => u.CPF == cpfUsuario);
                if (usuarioExistente == null)
                {
                    throw new ValidacaoPedidoException("Usuário com o CPF fornecido não encontrado");
                }

                Pedido novoPedido = new Pedido(id, itemExistente, qtdDesejada);

                if (novoPedido.QtdItens > itemExistente.Qtd)
                {
                    throw new ValidacaoPedidoException("Quantidade desejada maior que a quantidade em estoque");
                }

                pedidos.Add(novoPedido);
                itemExistente.Qtd -= novoPedido.QtdItens;
                usuarioExistente.ListaPedidos.Add(novoPedido);

                return Ok(pedidos);
            }
            catch (ValidacaoPedidoException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao processar a solicitação: " + ex.Message);
            }
        }

        [HttpPost("Item/CriarItem/{nomeItem}/{qtdDeEstoque}")]
        public IActionResult InserirItem([Required] string nomeItem, [Required] int qtdDeEstoque)
        {
            try
            {
                //Verifica se o nome do item é nulo ou vazio
                if (string.IsNullOrWhiteSpace(nomeItem))
                {
                    // O nome do item não pode ser nulo ou vazio
                    throw new ValidacaoItemException("O nome do item não pode estar vazio.");
                }

                //Verifica se o nome do item já existe na lista de itens à venda
                if (itensLoja.Any(item => item.NomeItem == nomeItem))
                {
                    //O nome do item já existe
                    throw new ValidacaoItemException("Nome do item já existe na loja.");
                }

                //Verifica se a quantidade de estoque é não negativa
                if (qtdDeEstoque < 0)
                {
                    //A quantidade de estoque não pode ser negativa
                    throw new ValidacaoItemException("A quantidade de estoque não pode ser negativa.");
                }

                //Cria um novo item e o adiciona à lista de itens à venda
                Itens novoItem = new Itens(nomeItem, qtdDeEstoque);
                itensLoja.Add(novoItem);

                //Retorna a lista de itens à venda atualizada
                return Ok(itensLoja);
            }
            catch (ValidacaoItemException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Erro ao processar a solicitação");
            }
        }

        [HttpPut("Usuario/AtualizarUsuario/{cpf}/{cpfNovo}/{nome}")]
        public IActionResult AtualizarUsuario([Required] string cpf, [Required] string nome, [Required] string cpfNovo)
        {
            try
            {
                //Verificar se o CPF é válido (contém exatamente 11 dígitos)
                if (string.IsNullOrEmpty(cpf) || cpf.Length != 11 || !cpf.All(char.IsDigit))
                {
                    throw new ValidacaoUsuarioException("O campo de CPF está vazio, inválido ou não possui 11 dígitos.");
                }

                //Verificar se o nome não contém números
                if (string.IsNullOrEmpty(nome) || nome.Any(char.IsDigit))
                {
                    throw new ValidacaoUsuarioException("O nome não pode conter números.");
                }

                //Verificar se o novo CPF é válido (contém exatamente 11 dígitos)
                if (string.IsNullOrEmpty(cpfNovo) || cpfNovo.Length != 11 || !cpfNovo.All(char.IsDigit))
                {
                    throw new ValidacaoUsuarioException("O novo CPF está vazio, inválido ou não possui 11 dígitos.");
                }

                // Verificar se o usuário existe com base no CPF
                var usuarioExistente = usuarios.FirstOrDefault(u => u.CPF == cpf);

                //Verificar se o usuário existe
                if (usuarioExistente == null)
                {
                    //Retorna um código 404 (Not Found) se o usuário não for encontrado
                    return NotFound("Usuário não encontrado.");
                }

                //Atualizar as propriedades do usuário com base nos dados fornecidos
                usuarioExistente.Nome = nome;
                usuarioExistente.CPF = cpfNovo;

                return NoContent();
            }
            catch (ValidacaoUsuarioException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao processar a solicitação: " + ex.Message);
            }
        }


        [HttpPut("Pedido/AtualizaPedido/{id}/{idAtualizado}/{itemAtualizado}/{qtdItensAtualizada}")]
        public IActionResult AtualizarPedido(string id, string idAtualizado, string itemAtualizado, int qtdItensAtualizada)
        {
            try
            {
                //Verifica se o pedido com o ID fornecido existe
                var pedidoAntigo = pedidos.FirstOrDefault(pedido => pedido.Id == id);

                if (pedidoAntigo == null)
                {
                    //Retorna um código 404 (Not Found) se o pedido não for encontrado
                    throw new ValidacaoPedidoException("Pedido não encontrado.");
                }

                //Verifica se o ID do pedido foi alterado para um valor já existente
                if (pedidos.Any(pedido => pedido.Id != id && pedido.Id == idAtualizado))
                {
                    // Retorna um código 400 indicando que o ID do pedido já foi utilizado
                    throw new ValidacaoPedidoException("ID do pedido já utilizado");
                }

                //Verifica se o item desejado está na lista de itens à venda
                var itemExistente = itensLoja.FirstOrDefault(item => item.NomeItem == itemAtualizado);
                if (itemExistente == null)
                {
                    //O item não está disponível para venda
                    throw new ValidacaoPedidoException("Item não encontrado na loja");
                }

                //Subtrai a quantidade anterior do estoque do itemAntigo
                var itemAntigo = itensLoja.FirstOrDefault(item => item.NomeItem == pedidoAntigo.Item.NomeItem);
                if (itemAntigo != null)
                {
                    itemAntigo.Qtd += pedidoAntigo.QtdItens;
                }

                // Verifica se a quantidade desejada é menor ou igual à quantidade em estoque
                if (qtdItensAtualizada > (itemExistente.Qtd + itemAntigo.Qtd))
                {
                    //A quantidade desejada é maior que a quantidade em estoque
                    throw new ValidacaoPedidoException("Quantidade desejada maior que a quantidade em estoque");
                }

                //Subtrai a quantidade desejada do estoque do itemExistente
                itemExistente.Qtd -= qtdItensAtualizada;

                pedidoAntigo.Id = idAtualizado;
                pedidoAntigo.Item = itemExistente;
                pedidoAntigo.QtdItens = qtdItensAtualizada;

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao processar a solicitação: " + ex.Message);
            }
        }

        [HttpPut("Item/AtualizarItem/{nome}")]
        public IActionResult AtualizarItem(string nome, [Required] string novoNome, [Required] int novaQuantidade)
        {
            try
            {
                //Procurar o item existente com base no nome
                var itemAntigo = itensLoja.FirstOrDefault(i => i.NomeItem == nome);

                //Verificar se o item existe
                if (itemAntigo == null)
                {
                    throw new ValidacaoItemException("Item não encontrado.");
                }

                //Verificar se o novo nome é único
                if (itensLoja.Any(i => i.NomeItem == novoNome && i != itemAntigo))
                {
                    throw new ValidacaoItemException("O novo nome do item já está em uso.");
                }

                //Atualizar as propriedades do item com base nos dados fornecidos
                itemAntigo.NomeItem = novoNome;
                itemAntigo.Qtd = novaQuantidade;

                return NoContent();
            }
            catch (ValidacaoItemException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao processar a solicitação: " + ex.Message);
            }
        }

        [HttpDelete("Usuario/DeletarUsuario/{cpfUsuario}")]
        public IActionResult RemoverUsuario([Required] string cpfUsuario)
        {
            try
            {
                //Verificar se o CPF é válido (contém apenas dígitos)
                if (string.IsNullOrEmpty(cpfUsuario))
                {
                    throw new ValidacaoUsuarioException("O campo de cpf deve ser preenchido");
                }

                if (!cpfUsuario.All(char.IsDigit))
                {
                    throw new ValidacaoUsuarioException("Apenas digitos são permitidos no campo de CPF");
                }
                
                if(cpfUsuario.Length != 11) {
                    throw new ValidacaoUsuarioException("CPF deve ter 11 digitos");
                }

                //Procurar o usuário existente com base no CPF
                var usuarioExistente = usuarios.FirstOrDefault(u => u.CPF == cpfUsuario);

                //Verificar se o usuário existe
                if (usuarioExistente == null)
                {
                    // Retorna um código 404 se o usuário não for encontrado
                    throw new ValidacaoUsuarioException("Usuário não encontrado.");
                }

                //Remover o usuário da lista
                usuarios.Remove(usuarioExistente);

                return NoContent();
            }
            catch (ValidacaoUsuarioException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao processar a solicitação: " + ex.Message);
            }
        }

        [HttpDelete("Pedido/DeletarPedido/{Id}/{cpfUsuario}")]
        public IActionResult RemoverPedido([Required] string Id, [Required] string cpfUsuario)
        {
            try
            {
                //Verificar se o ID do pedido e o CPF são válidos
                if (string.IsNullOrEmpty(Id))
                {
                    throw new ValidacaoPedidoException("O campo de ID não pode estar vazio");
                }

                if (string.IsNullOrEmpty(cpfUsuario))
                {
                    throw new ValidacaoPedidoException("O campo de CPF não pode estar vazio");
                }

                if (!Id.All(char.IsDigit))
                {
                    throw new ValidacaoPedidoException("ID do pedido deve ser preenchido com digitos");
                }

                if (!cpfUsuario.All(char.IsDigit))
                {
                    throw new ValidacaoPedidoException("O CPF do usuario deve ser preenchido com digitos");
                }

                if(cpfUsuario.Length!=11)
                {
                    throw new ValidacaoPedidoException("O CPF do usuario deve ter 11 digitos");
                }

                //Procurar o usuário existente com base no CPF
                var usuarioExistente = usuarios.FirstOrDefault(u => u.CPF == cpfUsuario);
                
                //Verificar se o usuário existe
                if (usuarioExistente == null)
                {
                    throw new ValidacaoPedidoException("Usuário não encontrado.");
                }

                var pedidoExistente = usuarioExistente.ListaPedidos.FirstOrDefault(p => p.Id == Id);

                //Verificar se o pedido existe para o usuário
                if (pedidoExistente == null)
                {
                    throw new ValidacaoPedidoException("Pedido não encontrado para o usuário especificado.");
                }

                //Remover o pedido da lista de pedidos do usuário
                usuarioExistente.DeletarPedido(Id);

                //Remover o pedido da lista geral de pedidos
                pedidos.Remove(pedidoExistente);

                // Retorna um código 204 indicando sucesso na remoção
                return NoContent();
            }
            catch (ValidacaoPedidoException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao processar a solicitação: " + ex.Message);
            }
        }

        [HttpDelete("Item/DeletarItem/{nome}")]
        public IActionResult RemoverItem([Required] string nome)
        {
            try
            {
                //Verificar se o nome do item é válido
                if (string.IsNullOrWhiteSpace(nome))
                {
                    throw new ValidacaoItemException("O campo do nome deve estar preenchido");
                }

                //Procurar o item existente com base no nome
                var itemExistente = itensLoja.FirstOrDefault(u => u.NomeItem == nome);

                //Verificar se o item existe
                if (itemExistente == null)
                {
                    throw new ValidacaoItemException("Item não encontrado na loja.");
                }

                //Remover o item da lista
                itensLoja.Remove(itemExistente);

                //Retorna um código 204 indicando sucesso na remoção
                return NoContent();
            }
            catch (ValidacaoItemException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao processar a solicitação: " + ex.Message);
            }
        }
    }
}