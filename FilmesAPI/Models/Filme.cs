using System.ComponentModel.DataAnnotations;

namespace FilmesAPI.Models;

public class Filme
{
    [Key]
    [Required]
    public int Id { get; set; }
    [Required(ErrorMessage = "O titulo do filme é obrigatório.")] //Define que a propriedade é obrigatória
    [StringLength(50)] // limita a entrada a no maximo 50 caracteres
    public string Titulo { get; set; }
    [Required(ErrorMessage = "É necessário informar o Gênero do filme.")] //Define que a propriedade é obrigatória
    [MaxLength(50, ErrorMessage = "O numero maximo de caracteres para o genero do filme é 50.")] // limita a entrada a no maximo 50 caracteres?!?!?!? (qual a diferença para o [StringLength]?)
    public string Genero { get; set; }
    [Required]
    [Range(70, 300, ErrorMessage = "A duração do filme deve ter entre 70 e 300 minutos")]
    public int Duracao { get; set; }
}

