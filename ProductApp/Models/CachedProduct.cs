using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace ProductApp.Models
{
    [SQLite.Table("Products")]
    public class CachedProduct
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime CachedAt { get; set; }
    }
}
