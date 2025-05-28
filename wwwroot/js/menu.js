async function sendRequest(option) {
    const output = document.getElementById("output");
    output.innerHTML = "⌛ Loading...";

    let endpoint = `/api/operation/option/${option}`;
    let headers = { "Content-Type": "text/plain" };
    let body = "";

    if (option === 5 || option === 7) {
        const name = prompt("Enter terrorist name:");
        if (!name) return;
        body = name;
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
            const json = JSON.parse(text);
            displayJsonAsTable(json);
        } catch (e) {
            if (text.includes("===") && text.includes("|")) {
                displayAllTables(text);
            } else if (text.includes("===")) {
                displayReport(text);
            } else {
                output.textContent = text;
            }
        }
    } catch (err) {
        output.textContent = "⚠️ Error: " + err.message;
    }
}

// 🆕 תצוגת JSON בטבלה
function displayJsonAsTable(data) {
    const output = document.getElementById("output");
    output.innerHTML = ""; // ננקה לפני הצגה

    // אם זה אובייקט עם מערכים בפנים – ניצור טבלה עבור כל מפתח
    if (!Array.isArray(data) && typeof data === "object") {
        for (const key in data) {
            if (Array.isArray(data[key])) {
                output.innerHTML += `<h2>${key}</h2>`;
                output.innerHTML += buildTableFromArray(data[key]);
            }
        }
        return;
    }

    // אם מדובר במערך פשוט
    if (Array.isArray(data)) {
        output.innerHTML = buildTableFromArray(data);
        return;
    }

    // אחרת – נדפיס אותו כטקסט פשוט
    output.textContent = JSON.stringify(data, null, 2);
}

function buildTableFromArray(arr) {
    if (!arr || arr.length === 0) return "<p>No data available.</p>";

    const headers = Object.keys(arr[0]);
    let html = "<table><thead><tr>";
    headers.forEach(h => {
        html += `<th>${h}</th>`;
    });
    html += "</tr></thead><tbody>";

    arr.forEach(item => {
        html += "<tr>";
        headers.forEach(h => {
            const val = item[h];
            html += `<td>${Array.isArray(val) ? val.join(", ") : val}</td>`;
        });
        html += "</tr>";
    });

    html += "</tbody></table>";
    return html;
}
