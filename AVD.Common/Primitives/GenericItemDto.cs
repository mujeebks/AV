using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.Common.Primitives
{
    public enum ItemType {
        TEXTBOX, 
        TEXTAREA,
        DROPDOWN,
        MULTIDROPDOWN,
        CHECKBOX,
        RADIO,
        DATEPICKER,
        VISIBILITY,
        NOTAPPLICABLE
    }

    /// <summary>
    /// create a basic structure for  communicaitng betrween server and client 
    /// specific items that may be needed for specific operations. this is to be used for modeling
    /// specific data items that the cruise lines require for booking or fare code availability requests.
    /// Each instance of this class represents a specific item. A screen would potentially use a collection
    /// of there.
    ///
    /// </summary>
    public class GenericItemDto
    {
        /// <summary>
        /// type of this item
        /// </summary>
        public ItemType Type { get; set; }
        /// <summary>
        /// the message displayed to prompt the user for this item. this is should ideally be merged from
        /// some resource bundle.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// holds th evalue selected from the end user
        /// </summary>
        public string[] SelectedValue { get; set; }

        /// <summary>
        /// a name for this item. Example: pastPassenger or something. this should be used to key to the 
        /// resourcebundle.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Length of the field
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// signifies whether this item is mandatory or not
        /// </summary>
        public bool Mandatory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Option> Options { get; set; }

        /// <summary>
        /// Indicates if the items needs to be displayed or not
        /// </summary>
        public bool IsVisible { get; set; }
    }

    public class Option
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { set; get; }
    }
}
