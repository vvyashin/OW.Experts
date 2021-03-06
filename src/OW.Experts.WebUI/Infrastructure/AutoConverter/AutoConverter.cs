﻿using ExpressMapper;

namespace OW.Experts.WebUI.Infrastructure.AutoConverter
{
    public static class AutoConverter
    {
        private static IMappingServiceProvider _currentConverter;

        public static IMappingServiceProvider CurrentConverter
        {
            get => _currentConverter ?? Mapper.Instance;
            set => _currentConverter = value;
        }

        public static T ConvertTo<T>(this object source)
        {
            return (T)CurrentConverter.Map(source.GetType(), typeof(T), source);
        }

        public static void Reset()
        {
            _currentConverter = null;
        }
    }
}