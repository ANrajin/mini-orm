using orm.console.Entities;

namespace orm.console.Data
{
    public interface IORM<T> where T : IEntity<int>
    {
        public void GetAll();
        public void GetById(int id);
        public void Insert(T item);
        public void Update(T item);
        public void Delete(T item);
        public void Delete(int id);
    }
}
