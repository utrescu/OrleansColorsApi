using Orleans;
using System.Threading.Tasks;
using GrainInterfaces.States;

/// <summary>
/// Es crearà un grain per cada color a partir de la seva clau
/// en format string (que hauria de ser un codi RGB)
/// </summary>
namespace GrainInterfaces
{
    public interface IColorGrain : IGrainWithStringKey
    {
        Task<Color> GetColor();

        Task DeleteColor();

        Task AddTranslation(ColorTranslation translation);

        Task ModifyTranslation(ColorTranslation translation);

        Task DeleteTranslation(string translation);

    }
}
