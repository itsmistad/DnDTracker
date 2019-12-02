function randomIntFromInterval(min, max) {
    return ~~(Math.random() * (max - min + 1) + min);
}

function roll(amount, dN, dropLowest) {
    // Generate the specified amount of dN rolls.
    var results = [];
    for (var i = 0; i < amount; i++) {
        results.push(randomIntFromInterval(1, dN));
    }

    if (dropLowest) {
        // Find the lowest of the results.
        var lowest = results[0];
        var indexOfLowest = 0;
        results.forEach((n, i) => {
                if (n < lowest) {
                    indexOfLowest = i;
                    lowest = n;
                }
            });

        // Remove the lowest of the results.
        results.splice(indexOfLowest, 1);
    }

    return results;
}