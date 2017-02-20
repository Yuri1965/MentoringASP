using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RipperManagerLib
{
    public enum DownloadOptionDomain
    {
        Domain,     //качать ресурсы только в пределах домена
        DomainAll,  //качать ресурсы со всех доменов
    }

    public enum DownloadOptionLevel
    {
        Level0 = 0,     //качать только указанную страницу
        Level1 = 1,     //качать указанную страницу + следующий уровень
        Level2 = 2,     //качать указанную страницу + 2 следующих уровня
        LevelAll = -1,  //качать указанную страницу + все следующие уровни
    }

    public enum ContentTypes
    {
        ContentTextHTML,        //HTML содержимое
        ContentText,            //любое другое текстовое содержимое
        ContentStream,          //бинарное содержимое
        ContentNotSupported     //не поддерживаем закачку
    }
}
