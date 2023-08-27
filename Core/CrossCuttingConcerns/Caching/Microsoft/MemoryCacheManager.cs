using Core.Utilities.IoC;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;

namespace Core.CrossCuttingConcerns.Caching.Microsoft
{
    public class MemoryCacheManager : ICacheManager
    {
        //Adapter Pattern
        IMemoryCache _memoryCache;

        public MemoryCacheManager()
        {
            _memoryCache = ServiceTool.ServiceProvider.GetService<IMemoryCache>();
        }

        public void Add(string key, object value, int duration)
        {
            _memoryCache.Set(key, value, TimeSpan.FromMinutes(duration)); //ne kadar süre verirsek(duration) o kadar süre cache'de kalacak
        }

        public T Get<T>(string key) //cache'den belli bir türdeki veriyi getirir
        {
            return _memoryCache.Get<T>(key); 
        }

        public object Get(string key) //cache'den veriyi getirir ama dönecek tür object olduğu için tür dönüşümü yapılması gerekir
        {
            return _memoryCache.Get(key);
        }

        public bool IsAdd(string key) //belirli bir anahtar değeri cache'de var mı diye kontrol eder. varsa true yoksa false döner. (out _) ifadesi, değerin ne olduğunu döndürme sadece varlığını kontrol et demek.
        {
            return _memoryCache.TryGetValue(key, out _);
        }

        public void Remove(string key) //cache'deki veriyi siler
        {
            _memoryCache.Remove(key);
        }

        public void RemoveByPattern(string pattern) //verdiğimiz bir patterne göre çalışma anında cacheden silme işlemini yapar. örnek>> [CacheRemoveAspect("IProductService.Get)")]
        {
            var cacheEntriesCollectionDefinition = typeof(MemoryCache).GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance); //Bellekte MemoryCache türünde olan entriescollection(microsoftun bellekte tutma şekli ve yeri) datalarını bul
            var cacheEntriesCollection = cacheEntriesCollectionDefinition.GetValue(_memoryCache) as dynamic; // definitionu memory cache olanları bul
            List<ICacheEntry> cacheCollectionValues = new List<ICacheEntry>();

            foreach (var cacheItem in cacheEntriesCollection) //sonra her bir cache elemanını gez
            {
                ICacheEntry cacheItemValue = cacheItem.GetType().GetProperty("Value").GetValue(cacheItem, null);
                cacheCollectionValues.Add(cacheItemValue);
            }

            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase); // patterni bu sekilde olusturuyoruz. singleline olacak, compiled olacak, case sensitive olmayacak gibi
            var keysToRemove = cacheCollectionValues.Where(d => regex.IsMatch(d.Key.ToString())).Select(d => d.Key).ToList(); //>> yukarda gelen her bir cache datası içinde bu kurala uyanları keysToRemove de topla

            foreach (var key in keysToRemove) // uyanların keylerini tek tek bul
            {
                _memoryCache.Remove(key);  //onlarıda bellekten sil
            }
        }
    }
}
