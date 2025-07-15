function SendGetRequest(Controller, Method) {
    $.ajax({
        url: '/api/' + Controller + '/' + Method,
        type: 'GET',
        success: function (data) { return data; },
        error: function (xhr, status, error) { return new Error(error); }
    });
}



