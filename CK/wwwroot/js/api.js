const API_BASE_URL = '/api';

async function fetchWithAuth(url, options = {}) {
    options.credentials = 'include'; // Bắt buộc để gửi kèm Cookie
    options.headers = {
        'Content-Type': 'application/json',
        ...options.headers
    };

    const response = await fetch(`${API_BASE_URL}${url}`, options);

    if (response.status === 403) {
        alert('Forbidden: Bạn không có quyền truy cập dữ liệu này (IDOR Protected).');
    } else if (response.status === 400) {
        const error = await response.json();
        console.error('Tampering Detected / Validation Error:', error);
        alert('Dữ liệu không hợp lệ!');
    }
    return response;
}

// Hàm ví dụ được gọi từ index.html
async function submitBooking(bookingData, roomType) {
    const endpoint = roomType === 'Suite' ? '/bookings/suite' : '/bookings/standard';
    const res = await fetchWithAuth(endpoint, {
        method: 'POST',
        body: JSON.stringify(bookingData)
    });
    if (res.ok) alert('Đặt phòng thành công!');
}