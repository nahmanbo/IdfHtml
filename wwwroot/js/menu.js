// menu.js - handles various response formats including triplets and flat triplet sequences

async function sendRequest(option) {
    const output = document.getElementById("output");
    output.innerHTML = "⌛ Loading...";

    let endpoint = `/api/operation/option/${option}`;
    let headers = { "Content-Type": "text/plain" };
    let body = "";

    if (option === 5 || option === 7) {
        const id = prompt("Enter terrorist ID:");
        if (!id) return;
        body = id;
    } else if (option === 9) {
        const targetType = prompt("Enter target type:");
        if (!targetType) return;
        body = targetType;
    }

    try {
        const response = await fetch(endpoint, {
            method: 'POST',
            headers,
            body
        });

        const text = await response.text();
        console.log("📨 Server response:\n" + text);

        try {
            const parsed = JSON.parse(text);

            if (Array.isArray(parsed)) {
                // Flat array of triplet groups (title, subtitle, table or grouped data)
                if (parsed.length % 3 === 0 && typeof parsed[0] === "string" && typeof parsed[1] === "string") {
                    displaySequentialTriplets(parsed);
                }
                else if (parsed.length === 3 && typeof parsed[0] === "string" && isDictionaryOfArrays(parsed[2])) {
                    displayTripletWithGroupedTables(parsed);
                }
                else if (parsed.length === 3 && typeof parsed[0] === "string" && Array.isArray(parsed[2])) {
                    displayTripletWithArray(parsed);
                }
                else if (parsed.every(item => Array.isArray(item) && item.length === 3)) {
                    displayMultipleTriplets(parsed);
                }
                else {
                    output.innerHTML = `<pre>${JSON.stringify(parsed, null, 2)}</pre>`;
                }
            } else {
                output.innerHTML = `<pre>${JSON.stringify(parsed, null, 2)}</pre>`;
            }
        } catch (e) {
            output.textContent = text;
        }
    } catch (err) {
        output.textContent = "⚠️ Error: " + err.message;
    }
}

function isDictionaryOfArrays(obj) {
    if (!obj || typeof obj !== "object" || Array.isArray(obj)) return false;
    return Object.values(obj).every(value => Array.isArray(value));
}

function displayTripletWithArray([title, subtitle, data]) {
    const output = document.getElementById("output");
    output.innerHTML = `<h2>${title}</h2><h4>${subtitle}</h4>`;
    output.innerHTML += Array.isArray(data) && data.length > 0 ? buildTable(data) : `<p><em>No data</em></p>`;
}

function displayTripletWithGroupedTables([title, subtitle, grouped]) {
    const output = document.getElementById("output");
    output.innerHTML = `<h2>${title}</h2><h4>${subtitle}</h4>`;

    for (const group in grouped) {
        output.innerHTML += `<h3>${group}</h3>`;
        output.innerHTML += Array.isArray(grouped[group]) && grouped[group].length > 0 ? buildTable(grouped[group]) : `<p><em>No data for category</em></p>`;
    }
}

function displayMultipleTriplets(triplets) {
    const output = document.getElementById("output");
    output.innerHTML = "";

    triplets.forEach(([title, subtitle, data], i) => {
        output.innerHTML += `<h2>${title}</h2><h4>${subtitle}</h4>`;
        output.innerHTML += Array.isArray(data) && data.length > 0 ? buildTable(data) : `<p><em>No data</em></p>`;
        if (i < triplets.length - 1) output.innerHTML += `<hr/>`;
    });
}

function displaySequentialTriplets(flatArray) {
    const output = document.getElementById("output");
    output.innerHTML = "";

    for (let i = 0; i < flatArray.length; i += 3) {
        const title = flatArray[i];
        const subtitle = flatArray[i + 1];
        const data = flatArray[i + 2];

        output.innerHTML += `<h2>${title}</h2><h4>${subtitle}</h4>`;

        if (Array.isArray(data)) {
            output.innerHTML += buildTable(data);
        } else if (typeof data === "object" && data !== null) {
            for (const category in data) {
                output.innerHTML += `<h3>${category}</h3>`;
                output.innerHTML += buildTable(data[category]);
            }
        } else {
            output.innerHTML += `<p><em>No data</em></p>`;
        }

        if (i + 3 < flatArray.length) output.innerHTML += `<hr/>`;
    }
}

function buildTable(dataArray) {
    if (!Array.isArray(dataArray) || dataArray.length === 0) return "<p>No data available.</p>";

    const headers = Object.keys(dataArray[0]);
    let html = "<table><thead><tr>" + headers.map(h => `<th>${h}</th>`).join("") + "</tr></thead><tbody>";

    dataArray.forEach(row => {
        html += "<tr>" + headers.map(h => `<td>${Array.isArray(row[h]) ? row[h].join(", ") : row[h] ?? ""}</td>`).join("") + "</tr>";
    });

    html += "</tbody></table>";
    return html;
}

// ✅ TEST MODE (optional)
window.onload = function () {
    const testOutput = [
        "Firepower",
        "🔥 Weapons Available",
        {
            "vehicles": [
                { "Name": "Zik", "Ammo": "2/5" }
            ]
        },
        "Intelligence",
        "🧠 Status Overview",
        {
            "Alive": [
                { "Name": "Ahmed", "Id": 123 }
            ],
            "Dead": []
        }
    ];
    displaySequentialTriplets(testOutput);
};