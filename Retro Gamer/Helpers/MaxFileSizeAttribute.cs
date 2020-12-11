using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.Models
{
    /// <summary>
    /// Allows you to set a max size of a file to be uploaded, if the size of the file is over the maximum allowed size
    /// then it returns an error message
    /// </summary>
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Maximum allowed file size is { FirstDigitInBytes(_maxFileSize)} mb.";
        }

        /// <summary>
        /// Since bytes to Mb has lot of digit in it, this method convert the int number to a string then takes the first char
        /// which represent Megabytes. I.e: 2,097,152 Bytes is 2 MB
        /// </summary>
        private char  FirstDigitInBytes(int number)
        {
            //Convert int to string
            var convertNumber = Convert.ToString(number).ToString();
            //Takes the first char of the string
            var maxNumberMb = convertNumber.First();
            //Returns char
            return maxNumberMb;
        }
    }
}
