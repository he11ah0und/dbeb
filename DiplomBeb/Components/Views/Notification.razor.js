// Функция для создания нового уведомления
function createNotification(text) {
    // Создаем контейнер уведомления
    const notification = document.createElement('div');
    notification.classList.add('window');

    // Добавляем текст
    const heading = document.createElement('h3');
    heading.textContent = text;
    notification.appendChild(heading);

    // Вычисляем смещение для нового уведомления
    const offset = (document.querySelectorAll('.window').length * 8) + 'rem'; // Каждый элемент на 8rem ниже
    notification.style.setProperty('--offset', offset);

    // Добавляем уведомление в DOM
    document.body.appendChild(notification);

    // Удаляем уведомление после завершения анимации
    setTimeout(() => {
        notification.remove();
    }, 4600); // 4 секунды на отображение + 0.6 секунды на анимацию исчезновения
}

// Пример использования
createNotification('Первое уведомление');
setTimeout(() => createNotification('Второе уведомление'), 1000);
setTimeout(() => createNotification('Третье уведомление'), 2000);