$(document).ready(function() 
{
    var protectedPlayersInput = $('#Model_ProtectedPlayersIds');
    var protectedPlayersArray = [];
    if (protectedPlayersInput.val()) protectedPlayersArray = protectedPlayersInput.val().split(',').filter(id => id.trim() !== '');

    $('option[selected].player').onclick(element => {
        var index = protectedPlayersArray.indexOf(element.value);
        protectedPlayersArray.splice(index, 1);
        element.AddAttribute('selected');
    });

    $('option.player').onclick(element => {
        protectedPlayersArray.concat(element.value);
        element.AddAttribute('selected');
    });
});
/*document.querySelectorAll('[data-division]').forEach(tab => {
    tab.addEventListener('click', function(e) {
        const division = this.getAttribute('data-division');
        const targetId = this.getAttribute('href').substring(1);
        loadDivisionContent(division, targetId);
    });
});
function loadDivisionContent(division, targetId) 
{
    const targetElement = document.getElementById(targetId);
    targetElement.html(`
        <div class="loading-spinner d-flex justify-content-center p-4">
            <div class="spinner-border" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>
    `);
    $.ajax({
        url: '/api/Franchises/ProtectablePlayersHtml',
        type: 'GET',
        data: {
            division: division,
        },
        success: function (data) 
        { 
            targetElement.html(data); 
            document.getElementById(targetId+'-tab').removeEventListener('click');
        },
        error: function (xhr, status, error) 
        { 
            targetElement.html(`
                <div class="alert alert-danger" role="alert">
                    <h4 class="alert-heading">Error Loading Content</h4>
                    <p>Failed to load content for ${division.replace('_', ' ')}. Please try again.</p>
                    <button class="btn btn-outline-danger btn-sm" onclick="loadDivisionContent('${division}', '${targetId}')">
                        Retry
                    </button>
                </div>
            `);
        }
    });
}*/