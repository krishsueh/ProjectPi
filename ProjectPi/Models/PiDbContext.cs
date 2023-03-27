using System;
using System.Data.Entity;
using System.Linq;

namespace ProjectPi.Models
{
    public class PiDbContext : DbContext
    {
        // 您的內容已設定為使用應用程式組態檔 (App.config 或 Web.config)
        // 中的 'PiDbContext' 連接字串。根據預設，這個連接字串的目標是
        // 您的 LocalDb 執行個體上的 'ProjectPi.Models.PiDbContext' 資料庫。
        // 
        // 如果您的目標是其他資料庫和 (或) 提供者，請修改
        // 應用程式組態檔中的 'PiDbContext' 連接字串。
        public PiDbContext()
            : base("name=PiDbContext")
        {
        }

        // 針對您要包含在模型中的每種實體類型新增 DbSet。如需有關設定和使用
        // Code First 模型的詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=390109。

        public virtual DbSet<OrderRecord> OrderRecords { get; set; }
        public virtual DbSet<Appointment> Appointments { get; set; }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Counselor> Counselors { get; set; }
        public virtual DbSet<MainField> Fields { get; set; }
        public virtual DbSet<Feature> Features { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }

        public virtual DbSet<ChatRoom> ChatRooms { get; set; }

    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}