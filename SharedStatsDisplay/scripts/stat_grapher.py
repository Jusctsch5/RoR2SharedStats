#!/usr/bin/python
import pandas as pd
import plotly.graph_objects as go


import sys

print 'Number of arguments:', len(sys.argv), 'arguments.'
print 'Argument List:', str(sys.argv)

df = pd.read_csv(sys.argv[1])

fig = go.Figure(go.Scatter(x = df['AAPL_x'], y = df['AAPL_y'],
                  name='Share Prices (in USD)'))

fig.update_layout(title='Apple Share Prices over time (2014)',
                   plot_bgcolor='rgb(230, 230,230)',
                   showlegend=True)

fig.show()