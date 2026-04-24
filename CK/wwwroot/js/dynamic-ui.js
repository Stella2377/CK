document.addEventListener('DOMContentLoaded', () => {
    const roomTypeRadios = document.querySelectorAll('input[name="roomType"]');
    const suiteFields = document.getElementById('suiteFields');
    const airportCheckbox = document.getElementById('hasAirportTransfer');
    const flightFields = document.getElementById('flightFields');

    // Chuyển đổi loại phòng
    roomTypeRadios.forEach(radio => {
        radio.addEventListener('change', (e) => {
            if (e.target.value === 'Suite') {
                suiteFields.classList.remove('hidden');
            } else {
                suiteFields.classList.add('hidden');
            }
        });
    });

    // Toggle đưa đón sân bay
    airportCheckbox?.addEventListener('change', (e) => {
        if (e.target.checked) {
            flightFields.classList.remove('hidden');
        } else {
            flightFields.classList.add('hidden');
        }
    });
});