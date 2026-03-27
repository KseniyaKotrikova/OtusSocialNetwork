import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    // Начинаем с 1 пользователя, чтобы замерить чистую задержку (Latency)
    vus: 1,
    duration: '20s',
};

export default function () {
    // Подставьте ваш локальный порт (обычно 5001 или 5000)
    // Ищем популярную фамилию, чтобы заставить БД искать дольше
   // const url = 'http://localhost:5235/user/search?firstName=Ив&lastName=Ива';
   // const url = 'http://localhost:5235/user/search?firstName=%D0%98%D0%B2&lastName=%D0%98%D0%B2%D0%B0';
    //Юр Ян
    const url = 'http://localhost:5235/user/search?firstName=%D0%AE%D1%80&lastName=%D0%AF%D0%BD';

    const res = http.get(url);

    check(res, {
        'status is 200': (r) => r.status === 200,
    });

    // Небольшая пауза между запросами одного виртуального пользователя
    sleep(0.1);
}
