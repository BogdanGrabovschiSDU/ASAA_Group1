const express = require('express');
const fs = require('fs');
const app = express();
const PORT = 3000;

// Middleware to parse JSON bodies
app.use(express.json());

// Serve the HTML file
app.get('/', (req, res) => {
    res.sendFile(__dirname + '/index.html');
});

// Endpoint to handle form submission
app.post('/submit-order', (req, res) => {
    const bikeOrder = req.body;
    console.log('Received order:', bikeOrder);

    // Log the order to a file
    fs.appendFileSync('log.txt', `Order received: ${JSON.stringify(bikeOrder)}\n`);

    // Respond with a confirmation
    res.json({ message: 'Order received', order: bikeOrder });
});

// Serve the log file content
let latestLogData = ''; // Cache the latest log content

// Watch for changes in the log file
fs.watch('log.txt', (eventType, filename) => {
    if (filename && eventType === 'change') {
        // Update the latest log content when file changes
        latestLogData = fs.readFileSync('log.txt', 'utf8');
    }
});

// Endpoint to send the latest log content
app.get('/log', (req, res) => {
    res.send(latestLogData);
});

app.listen(PORT, () => {
    console.log(`Server is running on http://localhost:${PORT}`);
});
