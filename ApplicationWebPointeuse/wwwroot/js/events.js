$(document).ready(function () {
    $("[id*=mainCheckbox]").click(function () {
        if ($(this).is(':checked')) {
            $("[id*=subCheckbox]").prop('checked', true);
        } else {
            $("[id*=subCheckbox]").prop('checked', false);
        }
    });
});

$(document).ready(function () {
    $('[id^="info-"]').hover(function () {
        $(this).addClass(' fa-beat-fade');
    }, function () {
        $(this).removeClass(' fa-beat-fade');
    });
});

$(document).ready(function () {
    $('[id^="pdf-"]').hover(function () {
        $(this).addClass(' fa-shake');
    }, function () {
        $(this).removeClass(' fa-shake');
    });
});

document.addEventListener('DOMContentLoaded', function () {
    document.querySelector('#generate-pdf').addEventListener('click', function () {
        var checkboxes = document.querySelectorAll('[id*=subCheckbox]:checked');
        var id = [];
        checkboxes.forEach(function (checkbox) {
            id.push(checkbox.value);
        });
        if (id.length > 0) {
            var url = '/Students/DisplayStudentPDF';
            url += '?id=' + id.join(',');
            window.location.href = url;
        }
    });
});