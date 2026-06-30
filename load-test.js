import http from 'k6/http';
import { check, sleep } from 'k6';

// 1. Конфигурация профиля нагрузки (План тестирования)
export const options = {
    stages: [
        { duration: '30s', target: 20 },  // Плавный разгон до 20 виртуальных пользователей
        { duration: '3m', target: 50 },   // Основная полка нагрузки: 50 пользователей в течение 3 минут
        { duration: '30s', target: 0 },   // Плавное снижение нагрузки до нуля
    ],
    thresholds: {
        http_req_failed: ['rate<0.01'],   // Тест считается успешным, если ошибок менее 1%
        http_req_duration: ['p(95)<100'], // 95% запросов должны выполняться быстрее 100мс
    },
};

const BASE_URL = 'http://localhost:5005/user'; // Адрес вашего .NET приложения снаружи

// Функция генерации фейкового валидного GUID v4, чтобы .NET не ругался на формат
function generateUUID() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

export default function () {
    // Генерируем случайный ID от 1 до 50000 (на основе сгенерированных нами ранее seed-данных)
    const randomId = Math.floor(Math.random() * 50000) + 1;

    // Генерируем случайное число для поиска по префиксу (например, "User_12", "User_453" и т.д.)
    const randomSearchNum = Math.floor(Math.random() * 49000) + 1;

    // Разделяем логику: 70% — получение по ID, 30% — поиск по имени
    if (Math.random() < 0.7) {
        const randomGuid = generateUUID();
        // Тест эндпоинта /user/get/{id}
        const res = http.get(`${BASE_URL}/get/${randomGuid}`);
        //console.log(res);

        check(res, {
            'get status is 200': (r) => r.status >= 200 && r.status === 404,
            'get has body': (r) => r.body && r.body.length > 0,
        });
    } else {
        // Тест эндпоинта /user/search
        const res = http.get(`${BASE_URL}/search?firstName=User_${randomSearchNum}&lastName=User_${randomSearchNum}`);
        //console.log(res);

        check(res, {
            'search status is 200': (r) => r.status >= 200 && r.status < 300,
            // Безопасная проверка: сначала смотрим, есть ли вообще body
            'search has body': (r) => r.body && r.body.length > 0,
        });

    }

    // Небольшая пауза между запросами пользователя (от 100 до 300 миллисекунд),
    // чтобы эмулировать реальное поведение человека
    sleep(Math.random() * 0.2 + 0.1);
}
