// Importa o namespace responsável pelas validações de dados
// e formatação visual através de Data Annotations.
using System.ComponentModel.DataAnnotations;

namespace appReversotask.Models
{
    // ViewModel utilizada exclusivamente para capturar
    // e validar os dados de entrada na tela de Login.
    public class LoginViewModel
    {
        // Define que o CPF é obrigatório.
        // Caso o usuário não informe o CPF, será exibida
        // a mensagem personalizada.
        [Required(ErrorMessage = "O CPF é obrigatório.")]

        // Define o nome amigável que será exibido
        // nas Views Razor através do <label asp-for="Cpf">.
        [Display(Name = "CPF do Paciente")]

        // Armazena o CPF informado pelo paciente.
        // Inicializado como string vazia para evitar
        // avisos relacionados à nulidade.
        public string Cpf { get; set; } = string.Empty;
    }
}
