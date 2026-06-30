import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    vus: 10,           // 10 одновременных потоков вставки
    duration: '2m',    // тест идет 1 минуту
};

export default function () {
    const url = 'http://localhost:5005/user/create';
    const payload = JSON.stringify({
        name: 'ChaosUser',
        email: 'chaos@otus.ru'
    });
    const params = {
        headers: { 'Content-Type': 'application/json' },
    };

    const res = http.post(url, payload, params);

    // Считаем только успешные HTTP 200 вставки
    check(res, {
        'is status 200': (r) => r.status === 200,
    });

    sleep(0.05); // Небольшая пауза, чтобы контролировать поток
}
