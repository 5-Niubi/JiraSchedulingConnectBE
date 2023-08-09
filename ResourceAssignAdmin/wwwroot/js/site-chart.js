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
        options: {
            plugins: {
                title: {
                    display: true,
                    text: 'Custom Chart Title'
                }
            }
        }
    }

    let chart = new Chart(document.getElementById("userDonut"), configChart);
}

function drawUserLineChart(year, arrData, arrDataPre) {
    const labels = [
        'January',
        'February',
        'March',
        'April',
        'May',
        'June',
        'July',
        'August',
        'September',
        'October',
        'November',
        'December'
    ];
    const data = {
        labels: labels,
        datasets: [{
            label: 'New User',
            data: arrData,
            fill: false,
            borderColor: 'rgb(75, 192, 192)',
            tension: 0.1
        },
        {
            label: 'Premium User',
            data: arrDataPre,
            fill: false,
            borderColor: 'rgb(255, 205, 86)',
            tension: 0.1
        }
        ]
    };

    const configChart = {
        type: 'line',
        data: data,
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'top',
                },
                title: {
                    display: true,
                    text: `New User in ${year}`
                }
            }
        },
    };

    let chart = new Chart(document.getElementById("userLine"), configChart);
}