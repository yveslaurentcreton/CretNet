# TwemojiCountryFlags.woff2

A webfont containing the regional-indicator block (U+1F1E6–U+1F1FF) and
nothing else, so a flag emoji renders on platforms whose system fonts have
no flag glyphs.

Windows is the reason it is here. Chrome and Edge render a regional-indicator
pair as two boxed letters, because Segoe UI Emoji has no flags. Firefox on
the same machine looks right — it ships Twemoji Mozilla — which is exactly
the kind of difference that gets diagnosed as "works on my machine".

`cn-ui.css` declares it with a `unicode-range`, so the browser fetches it
only when such a character is actually rendered and every other glyph keeps
coming from the normal stack.

## Attribution

Built from **Twemoji**, © Twitter, Inc. and other contributors, licensed
under **CC-BY 4.0** (https://creativecommons.org/licenses/by/4.0/).

Obtained from the `country-flag-emoji-polyfill` package
(https://www.npmjs.com/package/country-flag-emoji-polyfill), version 0.1.8.

Vendored rather than linked to a CDN: a font that only loads for four
characters is not worth a third-party request on every page, and the file
must be available to a deployment that cannot reach the public internet.
