// ---------------------------------------------------------------------------------------------------------------------------------------------
// Swagger UI Override UI and beahvior
// ---------------------------------------------------------------------------------------------------------------------------------------------

var SWAGGER_UI = swaggerUi;
var SWASHBUCKLE_CONFIG = swashbuckleConfig;
var SWAGGER_CLIENT = SwaggerClient;
var JQUERY = $;

JQUERY(document).ready(function () {
    var content = JQUERY('.swagger-section');
    var baseUrl = SWASHBUCKLE_CONFIG.rootUrl; 

    var _setupSecurityInformation = function() {

        var controls = JQUERY('span:contains("Implementation Notes")');

        if (controls === undefined || controls === null || controls.length <= 0) {
            return;
        }

        for (var i = 0; i < controls.length; i++) {
            var ctl = controls[i];

            ctl.innerHTML = 'Security Information';
            ctl.innerText = 'Security Information';
        }
    };

    var _setInHeader = function(type, value) {
        switch (type) {
            case 'Basic':
            case 'Bearer':

                if (type === 'Basic') {
                    value = btoa(value);
                }

                SWAGGER_UI.api.clientAuthorizations.add('key', new SWAGGER_CLIENT.ApiKeyAuthorization('Authorization', type + ' ' + value, 'header'));
                break;
            default:
                SWAGGER_UI.api.clientAuthorizations.authz = {};
                break;
        }
    };

    var _setUpAuthentication = function(data) {
        var authenticationField = JQUERY('#input_apiKey');       
        var basicAuthenticationField = JQUERY('#basicAuthentication');

        if (basicAuthenticationField.length === 0) {
            basicAuthenticationField = JQUERY('<span id="basicAuthentication">' +
                '<div class="input"><input placeholder="Username" id="input_username" name="username" type="text" size="10"></div>' +
                '<div class="input"><input placeholder="Password" id="input_password" name="password" type="password" size="10"></div>' +
                '</span>');

            basicAuthenticationField.insertBefore('#api_selector div.input:last-child');
        }                              

        var userNameField = JQUERY('#input_username');
        var userPasswordField = JQUERY('#input_password');

        authenticationField.hide();
        userNameField.hide();
        userPasswordField.hide();

        var authenticationTypes = data.securityTypes.split(',');                
        var securitySelectionField = JQUERY('#authenticationType');

        if (securitySelectionField.length === 0) {
            securitySelectionField = JQUERY('<select class="input" id="authenticationType"></select>');

            JQUERY.each(authenticationTypes,
                function(i, item) {
                    securitySelectionField.append(JQUERY('<option>',
                        {
                            value: item,
                            text: item
                        }));
                });

            securitySelectionField.insertBefore(authenticationField);
        }

        if (authenticationTypes.length <= 1) {
            securitySelectionField.hide();
        }
       
        securitySelectionField.off();
        securitySelectionField.on('change',
            function() {
                var type = this.value;

                authenticationField.val('');
                userNameField.val('');
                userPasswordField.val('');

                if (type === 'Bearer') {
                    userNameField.hide();
                    userPasswordField.hide();
                    authenticationField.show();
                    authenticationField.attr('placeholder', 'Json web token');
                } else if (type === 'Basic') {
                    authenticationField.hide();
                    userNameField.show();
                    userPasswordField.show();                   
                    authenticationField.attr('placeholder', '');
                } else {
                    authenticationField.hide();
                    userNameField.hide();
                    userPasswordField.hide();
                    authenticationField.attr('placeholder', '');
                }

                _setInHeader(type, '');
            });

        userNameField.off();
        userNameField.on('change',
            function() {
                var type = securitySelectionField.val();
                var userName = this.value;
                var userPassword = userPasswordField.val();

                _setInHeader(type, userName + ':' + userPassword);
            });

        userPasswordField.off();
        userPasswordField.on('change',
            function() {
                var type = securitySelectionField.val();
                var userName = userNameField.val();
                var userPassword = this.value;

                _setInHeader(type, userName + ':' + userPassword);
            });

        authenticationField.off();
        authenticationField.on('change',
            function() {
                var type = securitySelectionField.val();
                var value = this.value;

                _setInHeader(type, value);
            });

        securitySelectionField.val(data.defaultSecurityType).change();
    };

    var _setupFavIcon = function(data) {
        if (data.icon16 === undefined || data.icon16 === null || data.icon16.trim().length === 0) {
            return;
        }

        if (data.icon32 === undefined || data.icon32 === null || data.icon32.trim().length === 0) {
            return;
        }

        var icon16 = JQUERY('#icon16');
        var icon32 = JQUERY('#icon32');

        if (icon16.length !== 1 || icon32.length !== 1) {
            return;
        }

        icon16.attr('href', data.icon16);
        icon32.attr('href', data.icon32);
    };

    var _setUiInformations = function(data) {
        var titleHeader = JQUERY('.logo__title');
        var linkHeader = JQUERY('#logo');
        var sectionHeader = JQUERY('#header');
        var buttonExplore = JQUERY('.header__btn');
        var logoImage = JQUERY('.logo__img');
        var body = JQUERY('body, this');
        var headings = JQUERY('[id^="resource_"] > .heading');

        var title = data.applicationName;

        document.title = title;

        if (data.bodyBackgroundColor !== undefined &&
            data.bodyBackgroundColor !== null &&
            data.bodyBackgroundColor.trim().length > 0) {
            body.attr('style', 'background-color: ' + data.bodyBackgroundColor + ' !important');
        }

        if (data.sectionBackgroundColor !== undefined &&
            data.sectionBackgroundColor !== null &&
            data.sectionBackgroundColor.trim().length > 0) {

            JQUERY.each(headings,
                function(index, value) {
                    var item = JQUERY(value);

                    item.attr('style', 'background-color: ' + data.sectionBackgroundColor + ' !important');
                }
            );
        }

        if (data.logo !== undefined && data.logo !== null && data.logo.trim().length > 0 && logoImage.length === 1) {
            logoImage.attr('src', data.logo);
        }

        _setupFavIcon(data);

        if (linkHeader.length === 1) {
            linkHeader.click(function(e) {
                e.preventDefault();
            });
        }

        if (titleHeader.length === 1) {
            titleHeader.html(title);
            titleHeader.attr('style', 'color:' + data.titleTextColor);
        }

        if (sectionHeader.length === 1) {
            sectionHeader.attr('style', 'background-color: ' + data.headerBackgroundColor);
        }

        if (buttonExplore.length === 1) {
            buttonExplore.attr('style', 'background-color: ' + data.buttonBackgroundColor + '; color:' + data.titleTextColor);

            buttonExplore.hover(function() {
                JQUERY(this).attr('style', 'background-color: ' + data.buttonBackgroundHoverColor + '; color:' + data.titleTextColor);
            }).mouseout(function () {
                JQUERY(this).attr('style', 'background-color: ' + data.buttonBackgroundColor + '; color:' + data.titleTextColor);
            });
        }

        _setUpAuthentication(data);
    };

    var _mergeData = function(data, custom) {
        var result = JQUERY.extend({}, data);

        for (var key in data) {
            if (data.hasOwnProperty(key) && custom.hasOwnProperty(key)) {
                result[key] = custom[key];
            }
        }

        if (custom.hasOwnProperty('logo')) {
            result['logo'] = custom['logo'];
        }

        if (custom.hasOwnProperty('icon16')) {
            result['icon16'] = custom['icon16'];
        }

        if (custom.hasOwnProperty('icon32')) {
            result['icon32'] = custom['icon32'];
        }

        return result;
    };

    var _setUp = function() {

        _setupSecurityInformation();

        JQUERY.ajax({            
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            url: baseUrl + '/SwaggerConfigurationSdk.json',
            dataType: 'json',
            success: function (data) {    
                JQUERY.ajax({            
                    type: 'GET',
                    contentType: 'application/json; charset=utf-8',
                    url: baseUrl + '/SwaggerConfiguration.json',
                    dataType: 'json',
                    success: function (custom) {
                        _setUiInformations(_mergeData(data, custom));
                        content.show();
                    },
                    error: function () {
                        _setUiInformations(data);
                        content.show();
                    }
                });
            },
            error: function () {
                content.show();
            }
        });
    };

    _setUp();    
});

