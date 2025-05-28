using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SACA_Common.Models
{
    public abstract class BaseModel
    {
        [Key]
        public virtual string id { get; set; } = Guid.NewGuid().ToString();
    }
    public abstract class CategoryBaseModel : ExtendModel
    {
        [Comment("Đã sử dụng")]
        public bool is_used { get; set; } = false;
        [Comment("Theo dõi")]
        public bool is_followed { get; set; } = true;
    }
    public abstract class ExtendModel : BaseModel
    {
        public DateTime? created_on { get; set; }
        public string? created_by { get; set; }
        public bool deleted { get; set; }
        public string? deleted_by { get; set; }
        public DateTime? deleted_on { get; set; }
        public string? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
    }
    public class file_properties : ExtendModel
    {
        [Comment("Khoá ngoại đến đối tượng sở hữu")]
        public string parent_id { get; set; } = null!;
        [Comment("Tên file gốc (Bao gồm cả định dạng)")]
        public string filename { get; set; } = null!;
        [Comment("Định dạng")]
        public string? extension { get; set; }
        [Comment("Dung lượng")]
        public long? length { get; set; }
        [Comment("Đường dẫn file tương đối (relative path)")]
        public string path { get; set; } = null!;
    }
    public class FileModel
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        [Comment("Tên file gốc (Bao gồm cả định dạng)")]
        public string filename { get; set; } = null!;
        [Comment("Định dạng")]
        public string extension { get; set; } = null!;
        [Comment("Đường dẫn file")]
        public string path { get; set; } = null!;
    }

    public static class entity_extensions
    {
        public static T Created<T>(this T entity, string actor_id) where T : ExtendModel
        {
            entity.created_on = DateTime.Now;
            entity.created_by = actor_id;
            entity.modified_on = DateTime.Now;
            entity.modified_by = actor_id;
            return entity;
        }

        public static T Modified<T>(this T entity, string actor_id) where T : ExtendModel
        {
            entity.modified_on = DateTime.Now;
            entity.modified_by = actor_id;
            return entity;
        }

        public static void Deleted<T>(ref T entity, string actor_id) where T : ExtendModel
        {
            entity.deleted = true;
            entity.deleted_by = actor_id;
            entity.deleted_on = DateTime.Now;
            entity.modified_on = DateTime.Now;
            entity.modified_by = actor_id;
        }

        public static T Deleted<T>(this T entity, string actor_id) where T : ExtendModel
        {
            entity.deleted = true;
            entity.deleted_by = actor_id;
            entity.deleted_on = DateTime.Now;
            entity.modified_on = DateTime.Now;
            entity.modified_by = actor_id;
            return entity;
        }
    }
}