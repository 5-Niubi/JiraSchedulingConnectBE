function drawUserDonutChart(totalUsersArray) {
    const data = {
        labels: [
            'Free',
            'Plus'
        ],
        datasets: [{
            label: 'My First Dataset',
            data: totalUsersArray,
            backgroundColor: [
                'rgb(54, 162, 235)',
                'rgb(255, 205, 86)'
            ],
            hoverOffset: 4
        }]
    };


    let configChart = {
        type: 'doughnut',
        data: data,
    }

    let chart = new Chart(document.getElementById("userDonut"), configChart);
}

function drawUserLineChart() {
    const labels = [
        'January',
        'February',
        'March',
        'April',
        'May',
        'June',
        'July'
    ];
    const data = {
        labels: labels,
        datasets: [{
            label: 'My First Dataset',
            data: [65, 59, 80, 81, 56, 55, 40],
            fill: false,
            borderColor: 'rgb(75, 192, 192)',
            tension: 0.1
        }]
    };

    const configChart = {
        type: 'line',
        data: data,
    };

    let chart = new Chart(document.getElementById("userLine"), configChart);
}