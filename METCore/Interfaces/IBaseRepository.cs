using METCore.Models;

namespace METCore.Interfaces
{
    public interface IBaseRepository<T> where T : BaseClass
    {
        #region Create
        /// <summary>
        /// Crea y guarda nuevo objeto T.
        /// </summary>
        /// <param name="obj">Nuevo objeto a guardar.</param>
        /// <returns>Número de objetos alterados en la BBDD.</returns>
        Task<int> CreateT(T obj);

        /// <summary>
        /// Crea y guarda una lista de nuevos objetos T.
        /// </summary>
        /// <param name="obj">Lista de nuevos objetos a guardar.</param>
        /// <returns>Número de objetos alterados en la BBDD.</returns>
        Task<int> CreateMultipleT(IEnumerable<T> objList);
        #endregion


        #region Get
        /// <summary>
        /// Busca un T por Id.
        /// </summary>
        /// <param name="id">Parámetro de búsqueda del T.</param>
        /// <returns>T con Id igual al parámetro, o Null en su defecto.</returns>
        Task<T?> GetTById(int id);

        /// <summary>
        /// Busca múltiples T por lista de Ids.
        /// </summary>
        /// <param name="Ids">Parámetro de búsqueda del T.</param>
        /// <returns>Lista de T cuyos Ids se encontraban en la lista.</returns>
        Task<IList<T>> GetManyTByIds(IList<int> Ids);

        /// <summary>
        /// Busca múltiples T por array de Ids.
        /// </summary>
        /// <param name="Ids">Parámetro de búsqueda del T.</param>
        /// <returns>Lista de T cuyos Ids se encontraban en el array.</returns>
        Task<IList<T>> GetManyTByIds(int[] Ids);

        /// <summary>
        /// Cuenta número de T por lista de Ids.
        /// </summary>
        /// <param name="Ids">Parámetro de búsqueda del T.</param>
        /// <returns>Número de T cuyos Ids se encontraban en la lista.</returns>
        Task<int> CountManyTByIds(IList<int> Ids);

        /// <summary>
        /// Cuenta número de T por array de Ids.
        /// </summary>
        /// <param name="Ids">Parámetro de búsqueda del T.</param>
        /// <returns>Número de T cuyos Ids se encontraban en el array.</returns>
        Task<int> CountManyTByIds(int[] Ids);
        #endregion


        #region Update
        /// <summary>
        /// Actualiza el T obj.
        /// </summary>
        /// <param name="obj">T a actualizar.</param>
        /// <returns>Número de objetos alterados en la BBDD.</returns>
        Task<int> UpdateT(T obj);
        #endregion


        #region Delete
        /// <summary>
        /// Borra el T obj.
        /// </summary>
        /// <param name="obj">T a borrar.</param>
        /// <returns>Número de objetos alterados en la BBDD.</returns>
        Task<int> DeleteT(T obj);
        #endregion
    }
}
