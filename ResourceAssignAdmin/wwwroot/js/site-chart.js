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