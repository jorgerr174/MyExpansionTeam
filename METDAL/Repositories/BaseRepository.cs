using METCore.Interfaces;
using METCore.Models;
using METDAL.Data;
using Microsoft.EntityFrameworkCore;

namespace METDAL.Repositories
{
    public abstract class BaseRepository<T>(ApplicationDbContext context) : IBaseRepository<T> where T : BaseClass
    {
        protected readonly ApplicationDbContext _context = context;


        #region Create
        /// <summary>
        /// Crea y guarda nuevo objeto T.
        /// </summary>
        /// <param name="obj">Nuevo objeto a guardar.</param>
        /// <returns>Número de objetos alterados en la BBDD.</returns>
        public async Task<int> CreateT(T obj)
        {
            await _context.Set<T>().AddAsync(obj);
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Crea y guarda una lista de nuevos objetos T.
        /// </summary>
        /// <param name="obj">Lista de nuevos objetos a guardar.</param>
        /// <returns>Número de objetos alterados en la BBDD.</returns>
        public async Task<int> CreateMultipleT(IEnumerable<T> objList)
        {
            await _context.Set<T>().AddRangeAsync(objList);
            return await _context.SaveChangesAsync();
        }
        #endregion Create


        #region Get
        /// <summary>
        /// Busca un T por Id.
        /// </summary>
        /// <param name="Id">Parámetro de búsqueda del T.</param>
        /// <returns>T con Id igual al parámetro, o Null en su defecto.</returns>
        public async Task<T?> GetTById(int Id)
        {
            return await _context.Set<T>().SingleOrDefaultAsync(u => u.Id == Id);
        }

        /// <summary>
        /// Busca múltiples T por lista de Ids.
        /// </summary>
        /// <param name="Ids">Parámetro de búsqueda del T.</param>
        /// <returns>Lista de T cuyos Ids se encontraban en la lista.</returns>
        public async Task<IList<T>> GetManyTByIds(IList<int> Ids)
        {
            return await _context.Set<T>().Where(u => Ids.Contains(u.Id)).ToListAsync();
        }

        /// <summary>
        /// Busca múltiples T por array de Ids.
        /// </summary>
        /// <param name="Ids">Parámetro de búsqueda del T.</param>
        /// <returns>Lista de T cuyos Ids se encontraban en el array.</returns>
        public async Task<IList<T>> GetManyTByIds(int[] Ids)
        {
            return await GetManyTByIds(Ids.ToList());
        }

        /// <summary>
        /// Cuenta número de T por lista de Ids.
        /// </summary>
        /// <param name="Ids">Parámetro de búsqueda del T.</param>
        /// <returns>Número de T cuyos Ids se encontraban en la lista.</returns>
        public async Task<int> CountManyTByIds(IList<int> Ids)
        {
            return await _context.Set<T>().CountAsync(u => Ids.Contains(u.Id));
        }

        /// <summary>
        /// Cuenta número de T por array de Ids.
        /// </summary>
        /// <param name="Ids">Parámetro de búsqueda del T.</param>
        /// <returns>Número de T cuyos Ids se encontraban en el array.</returns>
        public async Task<int> CountManyTByIds(int[] Ids)
        {
            return await CountManyTByIds(Ids.ToList());
        }
        #endregion


        #region Update
        /// <summary>
        /// Actualiza el T obj.
        /// </summary>
        /// <param name="obj">T a actualizar.</param>
        /// <returns>Número de objetos alterados en la BBDD.</returns>
        public async Task<int> UpdateT(T obj)
        {
            // otra opción con 2 T como argumentos =>
            //newObj.Id = obj.Id;
            //_context.Teams.Entry(obj).CurrentValues.SetValues(newObj);
            _context.Set<T>().Update(obj);
            return await _context.SaveChangesAsync();
        }
        #endregion


        #region Delete
        /// <summary>
        /// Borra el T obj.
        /// </summary>
        /// <param name="obj">T a borrar.</param>
        /// <returns>Número de objetos alterados en la BBDD.</returns>
        public async Task<int> DeleteT(T obj)
        {
            _context.Set<T>().Remove(obj);
            return await _context.SaveChangesAsync();
        }
        #endregion
    }
}
